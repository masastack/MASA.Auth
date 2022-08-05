using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class positionName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position",
                column: "Name",
                unique: true);
        }
    }
}
