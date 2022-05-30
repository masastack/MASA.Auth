using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class userUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdCard",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                schema: "auth",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "auth",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0 and Email!=''");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdCard",
                schema: "auth",
                table: "Users",
                column: "IdCard",
                unique: true,
                filter: "[IsDeleted] = 0 and IdCard!=''");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "auth",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[IsDeleted] = 0 and PhoneNumber!=''");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Name",
                schema: "auth",
                table: "Positions",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdCard",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Positions_Name",
                schema: "auth",
                table: "Positions");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "auth",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdCard",
                schema: "auth",
                table: "Users",
                column: "IdCard",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "auth",
                table: "Users",
                column: "PhoneNumber",
                filter: "[IsDeleted] = 0");
        }
    }
}
