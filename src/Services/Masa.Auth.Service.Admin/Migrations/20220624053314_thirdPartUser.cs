using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class thirdPartUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUsers_IdentityProvider_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUsers_IdentityProvider_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUsers_User_Creator",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUsers_User_Modifier",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUsers_User_UserId",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThirdPartyUsers",
                schema: "auth",
                table: "ThirdPartyUsers");

            migrationBuilder.RenameTable(
                name: "ThirdPartyUsers",
                schema: "auth",
                newName: "ThirdPartyUser",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUsers_UserId",
                schema: "auth",
                table: "ThirdPartyUser",
                newName: "IX_ThirdPartyUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUsers_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUser",
                newName: "IX_ThirdPartyUser_ThirdPartyIdpId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUsers_Modifier",
                schema: "auth",
                table: "ThirdPartyUser",
                newName: "IX_ThirdPartyUser_Modifier");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUsers_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUser",
                newName: "IX_ThirdPartyUser_IdentityProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUsers_Creator",
                schema: "auth",
                table: "ThirdPartyUser",
                newName: "IX_ThirdPartyUser_Creator");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThirdPartyUser",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_IdentityProvider_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "IdentityProviderId",
                principalSchema: "auth",
                principalTable: "IdentityProvider",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_IdentityProvider_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "ThirdPartyIdpId",
                principalSchema: "auth",
                principalTable: "IdentityProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_User_Creator",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "Creator",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_User_Modifier",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "Modifier",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_User_UserId",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_IdentityProvider_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_IdentityProvider_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_User_Creator",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_User_Modifier",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_User_UserId",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThirdPartyUser",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.RenameTable(
                name: "ThirdPartyUser",
                schema: "auth",
                newName: "ThirdPartyUsers",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUser_UserId",
                schema: "auth",
                table: "ThirdPartyUsers",
                newName: "IX_ThirdPartyUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUser_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUsers",
                newName: "IX_ThirdPartyUsers_ThirdPartyIdpId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUser_Modifier",
                schema: "auth",
                table: "ThirdPartyUsers",
                newName: "IX_ThirdPartyUsers_Modifier");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUser_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers",
                newName: "IX_ThirdPartyUsers_IdentityProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyUser_Creator",
                schema: "auth",
                table: "ThirdPartyUsers",
                newName: "IX_ThirdPartyUsers_Creator");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThirdPartyUsers",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUsers_IdentityProvider_IdentityProviderId",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "IdentityProviderId",
                principalSchema: "auth",
                principalTable: "IdentityProvider",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUsers_IdentityProvider_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "ThirdPartyIdpId",
                principalSchema: "auth",
                principalTable: "IdentityProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUsers_User_Creator",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "Creator",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUsers_User_Modifier",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "Modifier",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUsers_User_UserId",
                schema: "auth",
                table: "ThirdPartyUsers",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
