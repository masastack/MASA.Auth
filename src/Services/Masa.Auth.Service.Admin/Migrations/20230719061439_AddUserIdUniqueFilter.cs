using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddUserIdUniqueFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff",
                column: "UserId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff",
                column: "UserId",
                unique: true);
        }
    }
}
