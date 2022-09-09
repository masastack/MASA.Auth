using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class updateThirdPartyIdp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonKeyMap",
                schema: "auth",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MapAll",
                schema: "auth",
                table: "IdentityProvider",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonKeyMap",
                schema: "auth",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "MapAll",
                schema: "auth",
                table: "IdentityProvider");
        }
    }
}
