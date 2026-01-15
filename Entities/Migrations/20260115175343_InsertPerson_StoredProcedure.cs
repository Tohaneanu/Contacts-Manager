using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPersons = @"
                CREATE PROCEDURE [dbo].[InsertPersons]
                (@PersonID uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40),
                @DateOfBirth datetime2(7), @Gender varchar(10), @CountryID uniqueidentifier,
                @Address nvarchar(200), @ReceiveNewsLetters bit)
                AS BEGIN
                    INSERT INTO [dbo].[Persons](PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters)
                    VALUES (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters)
                END";

            migrationBuilder.Sql(sp_InsertPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPersons = @"
                DROP PROCEDURE [dbo].[InsertPersons]
                ";

            migrationBuilder.Sql(sp_InsertPersons);
        }
    }
}
