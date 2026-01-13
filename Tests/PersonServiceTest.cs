
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;

namespace Tests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;

        //constructor
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personService = new PersonService();
            _countriesService = new CountriesService();
            _outputHelper = testOutputHelper;
        }

        #region AddPerson
        //When PersonAddRequest is null, it should trow ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personService.AddPerson(request);
            });
        }

        //When PersonName is null, it should trow ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? request = new PersonAddRequest() { PersonName = null };


            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personService.AddPerson(request);
            });
        }

        //When you supply proper Person details, it should insert(add) the Person to the existing
        //list of persons and it should return an object of PersonResponse, witch includes with the newly generated person id
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Andrei",
                Address = "sample address",
                Email = "tohanadr@gmail.com",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-02"),
                ReceiveNewsLetters = true
            };

            //Act
            PersonResponse responseResponse = _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_from_GetAllPersons = _personService.GetAllPersons();

            //Assert
            Assert.True(responseResponse.PersonID != Guid.Empty);
            Assert.Contains(responseResponse, persons_from_GetAllPersons);
        }

        #endregion

        #region GetPersonByPersonID

        //if we supply null as PersonID, it should return null as PersponResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;
            //Acts
            PersonResponse? person_response_from_get_method = _personService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(person_response_from_get_method);
        }

        //If we supply a valid person id, it should return the matching person details as personResponse object
        [Fact]
        public void GetPersonByPersonID_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "China" };
            CountryResponse? country_response = _countriesService.AddCountry(country_request);
            PersonAddRequest? person_add_request = new PersonAddRequest()
            {
                PersonName = "Andrei",
                Address = "sample address",
                Email = "tohanadr@gmail.com",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-02"),
                ReceiveNewsLetters = false
            };
            PersonResponse person_response_from_add_request = _personService.AddPerson(person_add_request);
            //Acts
            PersonResponse? person_response_from_get_method = _personService.GetPersonByPersonID(person_response_from_add_request.PersonID);

            //Assert
            Assert.Equal(person_response_from_add_request, person_response_from_get_method);
        }
        #endregion

        #region GetAllPersons

        //The list of persons should be empty by default(before adding any persons)
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Acts
            List<PersonResponse> actual_persons_response_list = _personService.GetAllPersons();

            //Assert
            Assert.Empty(actual_persons_response_list);
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were aded
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse? country_response = _countriesService.AddCountry(country_request);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                PersonName = "Andrei",
                Address = "sample address1",
                Email = "tohanadr@gmail.com",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-02"),
                ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                PersonName = "Dumitru",
                Address = "sample address2",
                Email = "tohanadr@yahoo.com",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1999-01-02"),
                ReceiveNewsLetters = false
                },
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(_personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> actualPersonResponseList = _personService.GetAllPersons();
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in actualPersonResponseList)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            foreach (PersonResponse expected_person in person_list_from_add_person)
            {
                Assert.Contains(expected_person, actualPersonResponseList);
            }
        }

        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse? country_response1 = _countriesService.AddCountry(country_request1);
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Andora" };
            CountryResponse? country_response2 = _countriesService.AddCountry(country_request2);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                PersonName = "Andrei",
                Address = "sample address1",
                Email = "tohanadr@gmail.com",
                CountryID = country_response1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-02"),
                ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                PersonName = "Dumitru",
                Address = "sample address2",
                Email = "tohanadr@yahoo.com",
                CountryID = country_response2.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1999-01-02"),
                ReceiveNewsLetters = false
                },
                new PersonAddRequest()
                {
                PersonName = "Alis",
                Address = "sample address A",
                Email = "alis@yahoo.com",
                CountryID = country_response2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2004-01-02"),
                ReceiveNewsLetters = true
                },
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(_personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in persons_list_from_search)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            foreach (PersonResponse expected_person in person_list_from_add_person)
            {
                Assert.Contains(expected_person, persons_list_from_search);
            }
        }

        //First we will add few persons; and then we will search based on person name with some search string.It should return the matching person
        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse? country_response1 = _countriesService.AddCountry(country_request1);
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Andora" };
            CountryResponse? country_response2 = _countriesService.AddCountry(country_request2);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                PersonName = "Andrei",
                Address = "sample address1",
                Email = "tohanadr@gmail.com",
                CountryID = country_response1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-02"),
                ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                PersonName = "Dumitru",
                Address = "sample address2",
                Email = "tohanadr@yahoo.com",
                CountryID = country_response2.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1999-01-02"),
                ReceiveNewsLetters = false
                },
                new PersonAddRequest()
                {
                PersonName = "Maria",
                Address = "sample address A",
                Email = "alis@yahoo.com",
                CountryID = country_response2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2004-01-02"),
                ReceiveNewsLetters = true
                },
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(_personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "a");
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in persons_list_from_search)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            foreach (PersonResponse expected_person in person_list_from_add_person)
            {
                if (expected_person.PersonName != null && expected_person.PersonName.Contains("a", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(expected_person, persons_list_from_search);
                }

            }
        }
        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public void GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse? country_response1 = _countriesService.AddCountry(country_request1);
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Andora" };
            CountryResponse? country_response2 = _countriesService.AddCountry(country_request2);
            List<PersonAddRequest> persons_request_list = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                PersonName = "Andrei",
                Address = "sample address1",
                Email = "tohanadr@gmail.com",
                CountryID = country_response1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-02"),
                ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                PersonName = "Dumitru",
                Address = "sample address2",
                Email = "tohanadr@yahoo.com",
                CountryID = country_response2.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1999-01-02"),
                ReceiveNewsLetters = false
                },
                new PersonAddRequest()
                {
                PersonName = "Alis",
                Address = "sample address A",
                Email = "alis@yahoo.com",
                CountryID = country_response2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2004-01-02"),
                ReceiveNewsLetters = true
                },
            };

            List<PersonResponse> person_list_from_add_person = new List<PersonResponse>();
            foreach (var person in persons_request_list)
            {
                person_list_from_add_person.Add(_personService.AddPerson(person));
            }

            //print person_list_from_add_person
            _outputHelper.WriteLine("Expected:");
            person_list_from_add_person = person_list_from_add_person.OrderByDescending(temp => temp.PersonName).ToList();
            foreach (PersonResponse person in person_list_from_add_person)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            List<PersonResponse> allPersons = _personService.GetAllPersons();
            //Act
            List<PersonResponse> persons_list_from_sort = _personService.GetSortedPersons(allPersons,nameof(PersonResponse.PersonName), SortOrderOptions.DESC);
            //print actualPersonResponseList
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in persons_list_from_sort)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert
            for (int i = 0; i < person_list_from_add_person.Count; i++)
            {
                Assert.Equal(person_list_from_add_person[i],persons_list_from_sort[i]);
            }
        }

        #endregion
    }
}
