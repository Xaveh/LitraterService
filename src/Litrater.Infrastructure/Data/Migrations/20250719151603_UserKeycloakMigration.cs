using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Litrater.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserKeycloakMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.AddColumn<Guid>(
                name: "KeycloakUserId",
                table: "Users",
                schema: "litrater_web_api",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_Users_KeycloakUserId",
                table: "Users",
                schema: "litrater_web_api",
                column: "KeycloakUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_KeycloakUserId",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.DropColumn(
                name: "KeycloakUserId",
                table: "Users",
                schema: "litrater_web_api");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                schema: "litrater_web_api",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                schema: "litrater_web_api",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                schema: "litrater_web_api",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                schema: "litrater_web_api",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                schema: "litrater_web_api",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "Users",
                schema: "litrater_web_api",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                schema: "litrater_web_api",
                column: "Email",
                unique: true);
        }
    }
}
