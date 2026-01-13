
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        //contructor
        public PersonService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
        }
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse response = person.ToPersonResponse();
            response.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return response;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
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
            //add person object into _persons
            _persons.Add(person);

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }
            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);
            if (person == null)
            {
                return null;
            }
            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }
            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(temp =>
                  temp.DateOfBirth != null ? temp.DateOfBirth.Value.ToString("dd mmm yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.CountryID):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Country) ? temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(temp =>
                  !string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                default:
                    matchingPersons = allPersons; break;
            }
            return matchingPersons;

        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
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
    }
}
