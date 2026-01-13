
using Entities;
using ServiceContracts.Enums;
using System.Runtime.CompilerServices;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Person Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// Compares the curent object data with the parameter object
        /// </summary>
        /// <param name="obj">The PersoneResponse object to compare</param>
        /// <returns>True or false, indicating whether all person details are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null) return false; 
            if(obj.GetType() != typeof(PersonResponse)) return false;
            PersonResponse other = (PersonResponse)obj;
            return PersonID == other.PersonID
                && PersonName == other.PersonName
                && Email == other.Email
                && DateOfBirth == other.DateOfBirth
                && Gender == other.Gender
                && CountryID == other.CountryID
                && Address == other.Address
                && ReceiveNewsLetters == other.ReceiveNewsLetters
                && Age == other.Age;   
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}," +
                $" Date of Birth: {DateOfBirth?.ToString("dd MMM yyyy")}, Gender: {Gender}," +
                $" Country ID: {CountryID}, Country: {Country}, Address: {Address}, Receive News Letters: {ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                   PersonID = PersonID,
                   PersonName = PersonName,
                   Email = Email,
                   DateOfBirth = DateOfBirth,
                   Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!, true),
                   Address = Address,
                   ReceiveNewsLetters = ReceiveNewsLetters                   
            };
        }
    }

    public static class PersoneExtension
    {
        /// <summary>
        /// An extension method to convert an object of Person class into PersonResponse class
        /// </summary>
        /// <param name="person">The person object to convert</param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            //person =>PersonResponse
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Address = person.Address,
                CountryID = person.CountryID,
                Gender = person.Gender,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
            };
        }
    }
}
