
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
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
    }
}
