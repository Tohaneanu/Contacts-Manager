
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
    }
}
