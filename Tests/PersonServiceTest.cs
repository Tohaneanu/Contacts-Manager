
using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using Xunit.Abstractions;

namespace Tests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture _fixture;

        //constructor
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
            _countriesService = new CountriesService(null);
            _personService = new PersonService(null);

            _outputHelper = testOutputHelper;
        }

        #region AddPerson
        //When PersonAddRequest is null, it should trow ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.AddPerson(request);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When PersonName is null, it should trow ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.AddPerson(request);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper Person details, it should insert(add) the Person to the existing
        //list of persons and it should return an object of PersonResponse, witch includes with the newly generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            //Act
            PersonResponse responseResponse = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_from_GetAllPersons = await _personService.GetAllPersons();

            //Assert
            //Assert.True(responseResponse.PersonID != Guid.Empty);
            responseResponse.PersonID.Should().NotBe(Guid.Empty);
            //Assert.Contains(responseResponse, persons_from_GetAllPersons);
            persons_from_GetAllPersons.Should().Contain(responseResponse);
        }

        #endregion

        #region GetPersonByPersonID

        //if we supply null as PersonID, it should return null as PersponResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;
            //Acts
            PersonResponse? person_response_from_get_method = await _personService.GetPersonByPersonID(personID);

            //Assert
            //Assert.Null(person_response_from_get_method);
            person_response_from_get_method.Should().BeNull();
        }

        //If we supply a valid person id, it should return the matching person details as personResponse object
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response = await _countriesService.AddCountry(country_request);
            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response.CountryID).Create();
            PersonResponse person_response_from_add_request = await _personService.AddPerson(person_add_request);
            //Acts
            PersonResponse? person_response_from_get_method = await _personService.GetPersonByPersonID(person_response_from_add_request.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add_request, person_response_from_get_method);
            person_response_from_get_method.Should().Be(person_response_from_add_request);
        }
        #endregion

        #region GetAllPersons

        //The list of persons should be empty by default(before adding any persons)
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Acts
            List<PersonResponse> actual_persons_response_list = await _personService.GetAllPersons();

            //Assert
            //Assert.Empty(actual_persons_response_list);
            actual_persons_response_list.Should().BeEmpty();
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were aded
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response = await _countriesService.AddCountry(country_request);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response.CountryID).Create(),
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone1@example.com").With(temp => temp.CountryID, country_response.CountryID).Create()
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(await _personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> actualPersonResponseList = await _personService.GetAllPersons();
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in actualPersonResponseList)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            //foreach (PersonResponse expected_person in person_list_from_add_person)
            //{
            //    Assert.Contains(expected_person, actualPersonResponseList);
            //} 
            actualPersonResponseList.Should().BeEquivalentTo(person_list_from_add_person);
        }

        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response1 = await _countriesService.AddCountry(country_request1);
            CountryAddRequest country_request2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response2 = await _countriesService.AddCountry(country_request2);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                 _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response1.CountryID).Create(),
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone1@example.com").With(temp => temp.CountryID, country_response2.CountryID).Create(),
                  _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone2@example.com").With(temp => temp.CountryID, country_response2.CountryID).Create()
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(await _personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in persons_list_from_search)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            //foreach (PersonResponse expected_person in person_list_from_add_person)
            //{
            //    Assert.Contains(expected_person, persons_list_from_search);

            //}
            persons_list_from_search.Should().BeEquivalentTo(person_list_from_add_person);
        }

        //First we will add few persons; and then we will search based on person name with some search string.It should return the matching person
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_request1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response1 = await _countriesService.AddCountry(country_request1);
            CountryAddRequest country_request2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response2 = await _countriesService.AddCountry(country_request2);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                 _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Andreea").With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response1.CountryID).Create(),
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Andrei").With(temp => temp.Email, "someone1@example.com").With(temp => temp.CountryID, country_response2.CountryID).Create(),
                  _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Ion").With(temp => temp.Email, "someone2@example.com").With(temp => temp.CountryID, country_response2.CountryID).Create()
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(await _personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("All list:");
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "an");
            //print actualPersonResponseList
            _outputHelper.WriteLine("Person name contains 'an':");
            foreach (PersonResponse person in persons_list_from_search)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            //foreach (PersonResponse expected_person in person_list_from_add_person)
            //{
            //    if (expected_person.PersonName != null && expected_person.PersonName.Contains("a", StringComparison.OrdinalIgnoreCase))
            //    {
            //        Assert.Contains(expected_person, persons_list_from_search);
            //    }
            //}
            persons_list_from_search.Should().OnlyContain(temp => temp.PersonName != null && temp.PersonName.Contains("a", StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_request1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response1 = await _countriesService.AddCountry(country_request1);
            CountryAddRequest country_request2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response2 = await _countriesService.AddCountry(country_request2);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                 _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response1.CountryID).Create(),
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone1@example.com").With(temp => temp.CountryID, country_response2.CountryID).Create(),
                  _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone2@example.com").With(temp => temp.CountryID, country_response2.CountryID).Create()
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(await _personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            person_list_from_add_person = person_list_from_add_person.OrderByDescending(temp => temp.PersonName).ToList();
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            //Act
            List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in persons_list_from_sort)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            //for (int i = 0; i < person_list_from_add_person.Count; i++)
            //{
            //    Assert.Equal(person_list_from_add_person[i], persons_list_from_sort[i]);
            //}
            //persons_list_from_sort.Should().BeEquivalentTo(person_list_from_add_person);
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply invalid peronID, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When personName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response = await _countriesService.AddCountry(country_request);
            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response.CountryID).Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personService.UpdatePerson(person_update_request);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response = await _countriesService.AddCountry(country_request);
            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response.CountryID).Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "Will";
            person_update_request.Email = "w@sch.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);
            //Assert
            //Assert.Equal(person_response_from_update, person_response_from_get);
            person_response_from_update.Should().Be(person_response_from_get);
        }

        #endregion

        #region DeletePerson

        //If you supply an valid personId, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse? country_response = await _countriesService.AddCountry(country_request);
            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.CountryID, country_response.CountryID).Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            //Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assertion
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        //If you supply an invalid personId, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assertion
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}
