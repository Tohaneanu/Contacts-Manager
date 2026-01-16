using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _db;

        //contructor
        public CountriesService(ApplicationDbContext personsDbContext)
        {
            _db = personsDbContext;

        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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

            if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exist");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();
            //generate CountryID
            country.CountryID = Guid.NewGuid();
            //add country object into _countries
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;
            Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == countryID);
            if (country_response_from_list == null) { return null; }

            return country_response_from_list.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            int countriesInserted = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];
                int rowCount = excelWorksheet.Dimension.Rows;

                for (int i = 2; i <= rowCount; i++)
                {
                    string? cellValue = Convert.ToString(excelWorksheet.Cells[i, 1].Value);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string countryName = cellValue;
                        int countriesCount = await _db.Countries.Where(country => country.CountryName == countryName).CountAsync();
                        if (countriesCount == 0)
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };
                            _db.Countries.Add(country);
                            await _db.SaveChangesAsync();
                            countriesInserted++;
                        }
                    }
                }
            }
            return countriesInserted;
        }
    }
}
