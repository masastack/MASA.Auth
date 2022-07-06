using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class position : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff",
                column: "PositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff",
                column: "PositionId",
                unique: true,
                filter: "[PositionId] IS NOT NULL");
        }
    }
}
