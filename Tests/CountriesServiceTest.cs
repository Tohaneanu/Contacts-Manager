using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;

namespace Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;

        //contructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            _countriesService = new CountriesService(dbContext);
        }

        #region AddCountry
        //When CountryAddRequest is null, it should trow ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    //Act
            //    await _countriesService.AddCountry(request);
            //});
            Func<Task> action = async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When CountryName is null, it should trow ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, null as string).Create();

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When CountryName is duplicate, it should trow ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsDuplicate()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Romania").Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Romania").Create();

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper country name, it should insert(add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Romania").Create();

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

            //Assert
            //Assert.True(response.CountryID != Guid.Empty);
            response.CountryID.Should().NotBe(Guid.Empty);
            //Assert.Contains(response, countries_from_GetAllCountries);
            countries_from_GetAllCountries.Should().Contain(response);
        }

        #endregion

        #region GetAllCountries

        //The list of countries should be empty by default(before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            //Assert.Empty(actual_country_response_list);
            actual_country_response_list.Should().BeEmpty();
        }

        //The list of countries should be empty by default(before adding any countries)
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Romania").Create(),
                _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Italy").Create()
            };

            //Act
            List<CountryResponse> country_list_from_add_country = new List<CountryResponse>();
            foreach (var country in country_request_list)
            {
                country_list_from_add_country.Add(await _countriesService.AddCountry(country));
            }
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            //Assert
            //read each element from country_list_from_add_country
            //foreach (CountryResponse expected_country in country_list_from_add_country)
            //{
            //    Assert.Contains(expected_country, actualCountryResponseList);
            //}
            actualCountryResponseList.Should().BeEquivalentTo(country_list_from_add_country);
        }

        #endregion

        #region GetCountryByCountryID

        //if we supply null as CountryID, it should return null as CountryResponse
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Acts
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            //Assert.Null(country_response_from_get_method);
            country_response_from_get_method.Should().BeNull();
        }

        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "China").Create();
            CountryResponse country_response_from_add_request = await _countriesService.AddCountry(country_add_request);
            //Acts
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(country_response_from_add_request.CountryID);

            //Assert
            //Assert.Equal(country_response_from_add_request, country_response_from_get_method);
            country_response_from_get_method.Should().Be(country_response_from_add_request);
        }
        #endregion

    }
}
