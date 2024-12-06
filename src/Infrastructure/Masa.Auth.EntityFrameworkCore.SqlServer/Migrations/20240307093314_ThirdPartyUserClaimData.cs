using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class ThirdPartyUserClaimData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimData",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreationTime_ModificationTime",
                schema: "auth",
                table: "User",
                columns: new[] { "CreationTime", "ModificationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyUser_CreationTime_ModificationTime",
                schema: "auth",
                table: "ThirdPartyUser",
                columns: new[] { "CreationTime", "ModificationTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_CreationTime_ModificationTime",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyUser_CreationTime_ModificationTime",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropColumn(
                name: "ClaimData",
                schema: "auth",
                table: "ThirdPartyUser");
        }
    }
}
