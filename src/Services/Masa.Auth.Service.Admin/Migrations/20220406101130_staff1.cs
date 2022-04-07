using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class staff1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TeamStaff_StaffId",
                schema: "subjects",
                table: "TeamStaff",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamStaff_Staff_StaffId",
                schema: "subjects",
                table: "TeamStaff",
                column: "StaffId",
                principalSchema: "subjects",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamStaff_Staff_StaffId",
                schema: "subjects",
                table: "TeamStaff");

            migrationBuilder.DropIndex(
                name: "IX_TeamStaff_StaffId",
                schema: "subjects",
                table: "TeamStaff");
        }
    }
}
