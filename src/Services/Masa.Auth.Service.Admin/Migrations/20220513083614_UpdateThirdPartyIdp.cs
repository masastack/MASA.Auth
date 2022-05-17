using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UpdateThirdPartyIdp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "subjects",
                table: "IdentityProvider",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifyFile",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "VerifyFile",
                schema: "subjects",
                table: "IdentityProvider");
        }
    }
}
