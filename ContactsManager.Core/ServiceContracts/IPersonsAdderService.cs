using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Add a Person object to the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person object to add</param>
        /// <returns>Returns the Person object after adding it(including newly generated Person id)</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
