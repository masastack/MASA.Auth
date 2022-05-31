using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class position : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                schema: "subjects",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IdCard",
                schema: "subjects",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Name",
                schema: "subjects",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_PhoneNumber",
                schema: "subjects",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "subjects",
                table: "User",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdCard",
                schema: "subjects",
                table: "User",
                column: "IdCard",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                schema: "subjects",
                table: "User",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNumber",
                schema: "subjects",
                table: "User",
                column: "PhoneNumber",
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                schema: "subjects",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IdCard",
                schema: "subjects",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Name",
                schema: "subjects",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_PhoneNumber",
                schema: "subjects",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "subjects",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_IdCard",
                schema: "subjects",
                table: "User",
                column: "IdCard",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                schema: "subjects",
                table: "User",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNumber",
                schema: "subjects",
                table: "User",
                column: "PhoneNumber",
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
