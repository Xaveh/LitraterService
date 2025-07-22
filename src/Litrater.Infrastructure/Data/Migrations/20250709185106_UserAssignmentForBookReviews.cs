using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Litrater.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserAssignmentForBookReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "BookReviews",
                schema: "litrater_web_api",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookReviews",
                schema: "litrater_web_api");
        }
    }
}
