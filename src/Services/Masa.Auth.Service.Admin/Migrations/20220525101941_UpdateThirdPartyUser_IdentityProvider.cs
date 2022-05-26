using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UpdateThirdPartyUser_IdentityProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyUsers_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "IdentityProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUsers_IdentityProvider_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "IdentityProviderId",
                principalSchema: "auth",
                principalTable: "IdentityProvider",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUsers_IdentityProvider_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyUsers_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropColumn(
                name: "IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers");
        }
    }
}
