using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;

namespace Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        //contructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            //var countriesInitialData = new List<Country>() { };
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        //When CountryAddRequest is null, it should trow ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
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
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
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
        public async Task AddCountry_CountryNameIsDuplicate_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Romania").Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "Romania").Create();

            Country first_country = request1.ToCountry();
            Country second_country = request2.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);
            //Return null when GetCountryByCountryName is called
            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);
            await _countriesService.AddCountry(request1);
            //Act
            var action = async () =>
            {
                //Return first country when GetCountryByCountryName is called
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName("Romania")).ReturnsAsync(first_country);

                await _countriesService.AddCountry(request2);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper country name, it should insert(add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(country);

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);

            //Act
            CountryResponse country_from_add_country = await _countriesService.AddCountry(country_request);
            country_response.CountryID = country_from_add_country.CountryID;

            //Assert
            country_from_add_country.CountryID.Should().NotBe(Guid.Empty);
            country_from_add_country.Should().BeEquivalentTo(country_response);
        }

        #endregion

        #region GetAllCountries

        //The list of countries should be empty by default(before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmptyList()
        {
            //Arange
            List<Country> countries = new List<Country>();
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            //Assert.Empty(actual_country_response_list);
            actual_country_response_list.Should().BeEmpty();
        }

        //
        [Fact]
        public async Task GetAllCountries_AddFewCountries_ShouldHaveFewCountries()
        {
            //Arrange
            List<Country> country_list = new List<Country>() {
                _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create(),
                _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create()
            };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            //Assert
            //read each element from country_list_from_add_country
            //foreach (CountryResponse expected_country in country_list_from_add_country)
            //{
            //    Assert.Contains(expected_country, actualCountryResponseList);
            //}
            actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
        }

        #endregion

        #region GetCountryByCountryID

        //if we supply null as CountryID, it should return null as CountryResponse
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
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
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessfull()
        {
            //Arrange
            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();
            CountryResponse country_response_expected = country.ToCountryResponse();
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);
            //Acts
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(country.CountryID);

            //Assert
            //Assert.Equal(country_response_from_add_request, country_response_from_get_method);
            country_response_from_get_method.Should().Be(country_response_expected);
        }
        #endregion

    }
}
