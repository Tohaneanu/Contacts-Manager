using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        //contructor
        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country> {
                    new Country()
                    {
                        CountryID = Guid.Parse("DDDB2BBA-6F3F-435B-BE6A-FEBABEFC87FA"),
                        CountryName = "Romania"
                    },
                     new Country()
                    {
                        CountryID = Guid.Parse("36A415F5-263C-4BA4-BA9E-8626021B6B98"),
                        CountryName = "Germany"
                    },
                      new Country()
                    {
                        CountryID = Guid.Parse("B7921BD9-FFE2-43BB-A869-B3182C46FFD6"),
                        CountryName = "Italy"
                    },
                       new Country()
                    {
                        CountryID = Guid.Parse("B8432737-0689-4F3D-9E17-9B9D90B73A6D"),
                        CountryName = "USA"
                    },
                        new Country()
                    {
                        CountryID = Guid.Parse("1E91FE60-B4B4-4FE3-A0CA-1B910498C453"),
                        CountryName = "China"
                    },
                });
            }
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequet parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name already exist");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();
            //generate CountryID
            country.CountryID = Guid.NewGuid();
            //add country object into _countries
            _countries.Add(country);
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;
            Country? country_response_from_list = _countries.FirstOrDefault(country => country.CountryID == countryID);
            if (country_response_from_list == null) { return null; }

            return country_response_from_list.ToCountryResponse();
        }
    }
}
