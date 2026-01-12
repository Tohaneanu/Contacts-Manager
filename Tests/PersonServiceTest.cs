
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;

namespace Tests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonServiceTest()
        {
            _personService = new PersonService();
            _countriesService = new CountriesService();
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

        #region GetCountryByCountryID

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

        //#region GetAllCountries

        ////The list of countries should be empty by default(before adding any countries)
        //[Fact]
        //public void GetAllCountries_EmptyList()
        //{
        //    //Acts
        //    List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

        //    //Assert
        //    Assert.Empty(actual_country_response_list);
        //}

        ////The list of countries should be empty by default(before adding any countries)
        //[Fact]
        //public void GetAllCountries_AddFewCountries()
        //{
        //    //Arrange
        //    List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
        //    {
        //        new CountryAddRequest(){CountryName = "Romania"},
        //        new CountryAddRequest(){CountryName = "Italy"}
        //    };

        //    //Act
        //    List<CountryResponse> country_list_from_add_country = new List<CountryResponse>();
        //    foreach (var country in country_request_list)
        //    {
        //        country_list_from_add_country.Add(_countriesService.AddCountry(country));
        //    }
        //    List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

        //    //Assert
        //    //read each element from country_list_from_add_country
        //    foreach (CountryResponse expected_country in country_list_from_add_country)
        //    {
        //        Assert.Contains(expected_country, actualCountryResponseList);
        //    }
        //}

        //#endregion
    }
}
