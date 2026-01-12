
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Add a Person object to the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person object to add</param>
        /// <returns>Returns the Person object after adding it(including newly generated Person id)</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Returns all persons 
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        List<PersonResponse> GetAllPersons();

        /// <summary>
        /// Returns a Person object based on the given Person id
        /// </summary>
        /// <param name="personID">PersonId (guid) to search</param>
        /// <returns>Matching Person as PersonResponse object</returns>
        PersonResponse? GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>REturns all matching persons based on the given search field and search string</returns>
        List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
    }
}
