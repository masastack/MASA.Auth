using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class StaffPsoition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Position_PositionId",
                schema: "subjects",
                table: "Staff");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Position_PositionId",
                schema: "subjects",
                table: "Staff",
                column: "PositionId",
                principalSchema: "organizations",
                principalTable: "Position",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Position_PositionId",
                schema: "subjects",
                table: "Staff");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Position_PositionId",
                schema: "subjects",
                table: "Staff",
                column: "PositionId",
                principalSchema: "organizations",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
