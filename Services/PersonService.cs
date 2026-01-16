
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        //contructor
        public PersonService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
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
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            //with procedure:
            //_db.sp_InsertPerson(person);
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _db.Persons.Include("Country").ToListAsync();
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
            Person? person = await _db.Persons.FirstOrDefaultAsync(person => person.PersonID == personID);
            if (person == null)
            {
                return null;
            }
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp =>
                  temp.DateOfBirth != null ? temp.DateOfBirth.Value.ToString("dd mmm yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Country) ? temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                default:
                    matchingPersons = allPersons; break;
            }
            return matchingPersons;

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
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);
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
            await _db.SaveChangesAsync();
            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID.Equals(personID));
            if (person == null)
            {
                return false;
            }
            _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID.Equals(personID)));
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
