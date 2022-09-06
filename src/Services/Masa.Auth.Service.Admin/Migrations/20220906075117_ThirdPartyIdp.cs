using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class ThirdPartyIdp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentificationType",
                schema: "auth",
                table: "IdentityProvider");

            migrationBuilder.RenameColumn(
                name: "VerifyType",
                schema: "auth",
                table: "IdentityProvider",
                newName: "AuthenticationType");

            migrationBuilder.RenameColumn(
                name: "VerifyFile",
                schema: "auth",
                table: "IdentityProvider",
                newName: "UserInformationEndpoint");

            migrationBuilder.RenameColumn(
                name: "Url",
                schema: "auth",
                table: "IdentityProvider",
                newName: "TokenEndpoint");

            migrationBuilder.AddColumn<string>(
                name: "AuthorizationEndpoint",
                schema: "auth",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallbackPath",
                schema: "auth",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationEndpoint",
                schema: "auth",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "CallbackPath",
                schema: "auth",
                table: "IdentityProvider");

            migrationBuilder.RenameColumn(
                name: "UserInformationEndpoint",
                schema: "auth",
                table: "IdentityProvider",
                newName: "VerifyFile");

            migrationBuilder.RenameColumn(
                name: "TokenEndpoint",
                schema: "auth",
                table: "IdentityProvider",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "AuthenticationType",
                schema: "auth",
                table: "IdentityProvider",
                newName: "VerifyType");

            migrationBuilder.AddColumn<int>(
                name: "IdentificationType",
                schema: "auth",
                table: "IdentityProvider",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
