using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Litrater.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLitraterWebApiSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "litrater_web_api");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "litrater_web_api");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "Books",
                newSchema: "litrater_web_api");

            migrationBuilder.RenameTable(
                name: "BookReviews",
                newName: "BookReviews",
                newSchema: "litrater_web_api");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "Authors",
                newSchema: "litrater_web_api");

            migrationBuilder.RenameTable(
                name: "AuthorBooks",
                newName: "AuthorBooks",
                newSchema: "litrater_web_api");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "litrater_web_api",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Books",
                schema: "litrater_web_api",
                newName: "Books");

            migrationBuilder.RenameTable(
                name: "BookReviews",
                schema: "litrater_web_api",
                newName: "BookReviews");

            migrationBuilder.RenameTable(
                name: "Authors",
                schema: "litrater_web_api",
                newName: "Authors");

            migrationBuilder.RenameTable(
                name: "AuthorBooks",
                schema: "litrater_web_api",
                newName: "AuthorBooks");
        }
    }
}
