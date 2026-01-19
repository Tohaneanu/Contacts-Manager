
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Globalization;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonsRepository _personsRepository;

        //contructor
        public PersonService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //Validation: personAddRequet parameter can't be null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(personAddRequest);

            //Convert object from PersonAddRequest to Person type
            Person person = personAddRequest.ToPerson();
            //generate PersonID
            person.PersonID = Guid.NewGuid();
            //add person object into _db
            //without procedure
            await _personsRepository.AddPerson(person);
            //with procedure:
            //_db.sp_InsertPerson(person);
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
            //return _db.Persons.ToList().Select(temp => temp.ToPersonResponse()).ToList();
            // return _db.sp_GetAllPersons().Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }
            //Person? person = _db.Persons.Include("Country").FirstOrDefault(person => person.PersonID == personID);
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
            {
                return null;
            }
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                     temp.PersonName.Contains(searchString)),
                nameof(PersonResponse.Email) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                     temp.Email.Contains(searchString)),
                nameof(PersonResponse.DateOfBirth) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                     temp.DateOfBirth.Value.ToString("dd mmm yyyy").Contains(searchString)),
                nameof(PersonResponse.Gender) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                     temp.Gender.Contains(searchString)),
                nameof(PersonResponse.CountryID) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                     temp.Country.CountryName.Contains(searchString)),
                nameof(PersonResponse.Address) =>
                     await _personsRepository.GetFilteredPersons(temp =>
                     temp.Address.Contains(searchString)),
                _ => await _personsRepository.GetAllPersons()
            };
            return persons.Select(person => person.ToPersonResponse()).ToList();

        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return allPersons;
            }
            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                 => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                  => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                   => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                    => allPersons.OrderBy(temp => temp.Gender).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.Gender).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                     => allPersons.OrderBy(temp => temp.Country).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.Country).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                     => allPersons.OrderBy(temp => temp.Address).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.Address).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                     => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
               => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),
                _ => allPersons
            };
            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person opject to update
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            await _personsRepository.UpdatePerson(matchingPerson);
            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
            {
                return false;
            }

            await _personsRepository.DeletePersonByPersonID(personID.Value);
            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWriter.NextRecord();
            List<PersonResponse> persons = await GetAllPersons();
            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-mmm-dd"));
                }
                else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            //ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();
                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                    {
                        workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        workSheet.Cells[row, 3].Value = "";
                    }
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Country;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;
                    row++;
                }
                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
