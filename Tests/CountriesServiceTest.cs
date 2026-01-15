using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        //contructor
        public CountriesServiceTest()
        { 
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options)); 
        }

        #region AddCountry
        //When CountryAddRequest is null, it should trow ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;
            
            //Assert
            Assert.Throws<ArgumentNullException>(()  => 
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        //When CountryName is null, it should trow ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        //When CountryName is duplicate, it should trow ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsDuplicate()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Romania" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Romania" };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        //When you supply proper country name, it should insert(add) the country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Romania" };

            //Act
            CountryResponse response = _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }

        #endregion

        #region GetAllCountries

        //The list of countries should be empty by default(before adding any countries)
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        //The list of countries should be empty by default(before adding any countries)
        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest(){CountryName = "Romania"},
                new CountryAddRequest(){CountryName = "Italy"}
            };

            //Act
            List<CountryResponse> country_list_from_add_country = new List<CountryResponse>();  
            foreach (var country in country_request_list)
            {
                country_list_from_add_country.Add(_countriesService.AddCountry(country));
            }
            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //Assert
            //read each element from country_list_from_add_country
            foreach ( CountryResponse expected_country in country_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }

        #endregion

        #region GetCountryByCountryID

        //if we supply null as CountryID, it should return null as CountryResponse
        [Fact]
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Acts
            CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        [Fact]
        public void GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "China" };
            CountryResponse country_response_from_add_request = _countriesService.AddCountry(country_add_request);
            //Acts
            CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(country_response_from_add_request.CountryID);

            //Assert
            Assert.Equal(country_response_from_add_request, country_response_from_get_method);
        }
        #endregion

    }
}
