
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        //contructor
        public PersonService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
            if (initialize)
            {
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("172FDF1A-7EED-4ABB-A994-056C937952BA"),
                    PersonName = "Gordon",
                    Email = "gtoon0@csmonitor.com",
                    DateOfBirth = DateTime.Parse("1998-12-28"),
                    Gender = "Male",
                    Address = "47037 Hooker Avenue",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("DDDB2BBA-6F3F-435B-BE6A-FEBABEFC87FA")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("B21384B3-A93D-4BE8-8975-5ECF7CE8FF54"),
                    PersonName = "Binni",
                    Email = "bbuncom1@linkedin.com",
                    DateOfBirth = DateTime.Parse("2002-11-01"),
                    Gender = "Female",
                    Address = "016 Kensington Plaza",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("36A415F5-263C-4BA4-BA9E-8626021B6B98")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("5E16438A-F148-416E-B9D3-1FC9EAC4EB45"),
                    PersonName = "Nanci",
                    Email = "nbarbary2@epa.gov",
                    DateOfBirth = DateTime.Parse("1990-05-26"),
                    Gender = "Female",
                    Address = "15 Schiller Parkway",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("B7921BD9-FFE2-43BB-A869-B3182C46FFD6")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("5E16438A-F148-416E-B9D3-1FC9EAC4EB45"),
                    PersonName = "Ailbert",
                    Email = "afollows3@mtv.com",
                    DateOfBirth = DateTime.Parse("1999-03-18"),
                    Gender = "Male",
                    Address = "8191 Westerfield Street",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("B8432737-0689-4F3D-9E17-9B9D90B73A6D")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("D233EED8-A532-4CE3-830D-C6B4AF3421C2"),
                    PersonName = "Cesya",
                    Email = "ciacovuzzi4@istockphoto.com",
                    DateOfBirth = DateTime.Parse("2000-09-17"),
                    Gender = "Female",
                    Address = "208 Anniversary Alley",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("1E91FE60-B4B4-4FE3-A0CA-1B910498C453")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("F9B12DDF-C442-485E-90F1-5F933A723153"),
                    PersonName = "Audra",
                    Email = "alilleyman5@fc2.com",
                    DateOfBirth = DateTime.Parse("1996-11-15"),
                    Gender = "Female",
                    Address = "78327 Schmedeman Hill",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("1E91FE60-B4B4-4FE3-A0CA-1B910498C453")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("F82E724A-A9CF-470B-B963-FCD6BFA2481D"),
                    PersonName = "Jay",
                    Email = "jpelfer6@nyu.edu",
                    DateOfBirth = DateTime.Parse("1996-04-06"),
                    Gender = "Male",
                    Address = "80 Starling Terrace",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("1E91FE60-B4B4-4FE3-A0CA-1B910498C453")
                });
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("37339429-E583-41D5-8D86-43497CE8A89D"),
                    PersonName = "Rafe",
                    Email = "rwandtke7@creativecommons.org",
                    DateOfBirth = DateTime.Parse("1999-05-01"),
                    Gender = "Male",
                    Address = "037 Esch Crossing",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("1E91FE60-B4B4-4FE3-A0CA-1B910498C453")
                });
                /*
Stephan,sbownas8@unesco.org,1997-12-07,Male,8507 Sunnyside Drive,false
Nigel,ncolloby9@nbcnews.com,1993-04-30,Male,7 Steensland Park,false
Agustin,alaysona@epa.gov,1992-06-07,Male,6803 Mcguire Avenue,true
Jonis,jgreeningb@fastcompany.com,1993-05-28,Female,28355 Eagle Crest Center,true
Catarina,cbenboughc@facebook.com,1994-01-10,Female,92917 Columbus Pass,false
Tony,tmoodied@google.ca,1993-05-09,Female,4863 Nobel Drive,false
Clim,cnieasse@wordpress.com,2001-10-10,Male,3 Lake View Point,false 
                */
            }
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
            return _persons.Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
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
            return ConvertPersonToPersonResponse(person);
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

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person opject to update
            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
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

            return ConvertPersonToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? person = _persons.FirstOrDefault(temp => temp.PersonID.Equals(personID));
            if (person == null)
            {
                return false;
            }
            _persons.RemoveAll(temp => temp.PersonID.Equals(personID));
            return true;
        }
    }
}
