using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Litrater.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Indexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                schema: "litrater_web_api",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                schema: "litrater_web_api",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_BookReviews_CreatedDate",
                table: "BookReviews",
                schema: "litrater_web_api",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_BookReviews_UserId",
                table: "BookReviews",
                schema: "litrater_web_api",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookReviews_UserId_BookId",
                table: "BookReviews",
                schema: "litrater_web_api",
                columns: ["UserId", "BookId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_FirstName_LastName",
                table: "Authors",
                schema: "litrater_web_api",
                columns: ["FirstName", "LastName"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                schema: "litrater_web_api");

            migrationBuilder.DropIndex(
                name: "IX_Books_Title",
                table: "Books",
                schema: "litrater_web_api");

            migrationBuilder.DropIndex(
                name: "IX_BookReviews_CreatedDate",
                table: "BookReviews",
                schema: "litrater_web_api");

            migrationBuilder.DropIndex(
                name: "IX_BookReviews_UserId",
                table: "BookReviews",
                schema: "litrater_web_api");

            migrationBuilder.DropIndex(
                name: "IX_BookReviews_UserId_BookId",
                table: "BookReviews",
                schema: "litrater_web_api");

            migrationBuilder.DropIndex(
                name: "IX_Authors_FirstName_LastName",
                table: "Authors",
                schema: "litrater_web_api");
        }
    }
}
