
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

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

            //Validation: PersonName can't be null
            if (personAddRequest.PersonName == null)
            {
                throw new ArgumentException(nameof(personAddRequest.PersonName));
            }

            ///validations for rest of the data.... to do soon...

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
            throw new NotImplementedException();
        }
    }
}
