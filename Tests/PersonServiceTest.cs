
using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace Tests
{
    public class PersonServiceTest
    {
        private readonly IPersonsAdderService _personService;
        //private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture _fixture;

        //constructor
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            //var countriesInitialData = new List<Country>() { };
            //var personsInitialData = new List<Person>() { };
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            //dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
            //_countriesService = new CountriesService(null);
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonsAdderService>>();
            _personService = new PersonService(_personsRepository,loggerMock.Object, diagnosticContextMock.Object);

            _outputHelper = testOutputHelper;
        }

        #region AddPerson
        //When PersonAddRequest is null, it should trow ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
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
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();
            Person person = request.ToPerson();

            //when PersonsREpository.AddPerson is called, it has to return the same "person" object
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
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
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();
            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();
            //if we supply any argument value to the AddPerson method, it should return the same value
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            //Assert
            //Assert.True(responseResponse.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);
        }

        #endregion

        #region GetPersonByPersonID

        //if we supply null as PersonID, it should return null as PersponResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
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
        public async Task GetPersonByPersonID_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
            //Acts
            PersonResponse? person_response_from_get_method = await _personService.GetPersonByPersonID(person.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add_request, person_response_from_get_method);
            person_response_from_get_method.Should().Be(person_response_expected);
        }
        #endregion

        #region GetAllPersons

        //The list of persons should be empty by default(before adding any persons)
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Acts
            List<PersonResponse> actual_persons_response_list = await _personService.GetAllPersons();

            //Assert
            //Assert.Empty(actual_persons_response_list);
            actual_persons_response_list.Should().BeEmpty();
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were aded
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone1@example.com").With(temp => temp.Country, null as Country).Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            //print person_response_list_expected
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_response_list_expected)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> person_response_from_get = await _personService.GetAllPersons();
            //print person_response_from_get
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in person_response_from_get)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            //foreach (PersonResponse expected_person in person_list_from_add_person)
            //{
            //    Assert.Contains(expected_person, actualPersonResponseList);
            //} 
            person_response_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange  
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone1@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone3@example.com").With(temp => temp.Country, null as Country).Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_expected
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_response_list_expected)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

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
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        //Search based on person name with some search string.It should return the matching person
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange  
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(p => p.PersonName, "Sami").With(p => p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.PersonName, "Sarah").With(p => p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.PersonName, "John").With(p => p.Country, null as Country).Create()
            };
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync((Expression<Func<Person, bool>> predicate) => persons.Where(predicate.Compile()).ToList());

            List<PersonResponse> person_response_list_expected = persons.Where(p => p.PersonName!.Contains("Sa"))
                .Select(p => p.ToPersonResponse()).ToList();
            //print person_response_list_expected
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_response_list_expected)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "Sa");
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
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange  
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(p => p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Country, null as Country).Create()
            };
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> person_response_list_expected = persons.Select(p => p.ToPersonResponse()).ToList();

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            person_response_list_expected = person_response_list_expected.OrderByDescending(temp => temp.PersonName).ToList();
            foreach (PersonResponse person in person_response_list_expected)
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
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
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
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string).With(temp => temp.Gender, GenderOptions.Male.ToString())
                .With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create();
            PersonResponse person_response_expected = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

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
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person originalPerson = _fixture.Build<Person>()
                .With(p => p.PersonName, "Sam").With(p => p.Gender, GenderOptions.Male.ToString())
                .With(p => p.Email, "someone@example.com").With(p => p.Country, null as Country).Create();
            _personRepositoryMock.Setup(r => r.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(originalPerson);
            Person updatedPerson = new Person()
            {
                PersonID = originalPerson.PersonID,
                PersonName = "Will",
                Email = "w@sch.com",
                DateOfBirth = originalPerson.DateOfBirth,
                Gender = originalPerson.Gender,
                Address = originalPerson.Address,
                Country = originalPerson.Country,
                CountryID = originalPerson.CountryID,
                TIN = originalPerson.TIN,
                ReceiveNewsLetters = originalPerson.ReceiveNewsLetters
            };
            _personRepositoryMock.Setup(r => r.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(updatedPerson);
            PersonResponse person_response_expected = updatedPerson.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();
            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);
            //Assert
            //Assert.Equal(person_response_from_update, person_response_from_get);
            person_response_from_update.Should().BeEquivalentTo(person_response_expected);
        }

        #endregion

        #region DeletePerson

        //If you supply an valid personId, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(p => p.Gender, GenderOptions.Male.ToString()).With(p => p.Email, "someone@example.com").With(p => p.Country, null as Country).Create();

            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
            //Act
            bool isDeleted = await _personService.DeletePerson(person.PersonID);

            //Assertion
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        //If you supply an invalid personId, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ToBeFalse()
        {
            //Arrange
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync((Person?)null);


            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assertion
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}
