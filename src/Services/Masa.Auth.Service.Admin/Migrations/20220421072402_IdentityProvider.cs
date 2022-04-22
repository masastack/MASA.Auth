using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class IdentityProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_ThirdPartyIdp_ThirdPartyIdpId",
                schema: "subjects",
                table: "ThirdPartyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThirdPartyIdp",
                schema: "subjects",
                table: "ThirdPartyIdp");

            migrationBuilder.RenameTable(
                name: "ThirdPartyIdp",
                schema: "subjects",
                newName: "IdentityProvider",
                newSchema: "subjects");

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                schema: "permissions",
                table: "Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                schema: "permissions",
                table: "Role",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Limit",
                schema: "permissions",
                table: "Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifierUserId",
                schema: "permissions",
                table: "Role",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VerifyType",
                schema: "subjects",
                table: "IdentityProvider",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSecret",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BaseDn",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupSearchBaseDn",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSSL",
                schema: "subjects",
                table: "IdentityProvider",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootUserDn",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootUserPassword",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerAddress",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServerPort",
                schema: "subjects",
                table: "IdentityProvider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserSearchBaseDn",
                schema: "subjects",
                table: "IdentityProvider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityProvider",
                schema: "subjects",
                table: "IdentityProvider",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "subjects",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRole_RoleId",
                schema: "subjects",
                table: "TeamRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRelation_ParentId",
                schema: "permissions",
                table: "RoleRelation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatorUserId",
                schema: "permissions",
                table: "Role",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ModifierUserId",
                schema: "permissions",
                table: "Role",
                column: "ModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_Name",
                schema: "subjects",
                table: "IdentityProvider",
                column: "Name",
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_CreatorUserId",
                schema: "permissions",
                table: "Role",
                column: "CreatorUserId",
                principalSchema: "subjects",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_ModifierUserId",
                schema: "permissions",
                table: "Role",
                column: "ModifierUserId",
                principalSchema: "subjects",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_ParentId",
                schema: "permissions",
                table: "RoleRelation",
                column: "ParentId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamRole_Role_RoleId",
                schema: "subjects",
                table: "TeamRole",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_IdentityProvider_ThirdPartyIdpId",
                schema: "subjects",
                table: "ThirdPartyUser",
                column: "ThirdPartyIdpId",
                principalSchema: "subjects",
                principalTable: "IdentityProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "subjects",
                table: "UserRole",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_CreatorUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_ModifierUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_ParentId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamRole_Role_RoleId",
                schema: "subjects",
                table: "TeamRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_IdentityProvider_ThirdPartyIdpId",
                schema: "subjects",
                table: "ThirdPartyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "subjects",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_RoleId",
                schema: "subjects",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_TeamRole_RoleId",
                schema: "subjects",
                table: "TeamRole");

            migrationBuilder.DropIndex(
                name: "IX_RoleRelation_ParentId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.DropIndex(
                name: "IX_Role_CreatorUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_ModifierUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityProvider",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropIndex(
                name: "IX_IdentityProvider_Name",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Limit",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "BaseDn",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "GroupSearchBaseDn",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "IsSSL",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "RootUserDn",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "RootUserPassword",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "ServerAddress",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "ServerPort",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.DropColumn(
                name: "UserSearchBaseDn",
                schema: "subjects",
                table: "IdentityProvider");

            migrationBuilder.RenameTable(
                name: "IdentityProvider",
                schema: "subjects",
                newName: "ThirdPartyIdp",
                newSchema: "subjects");

            migrationBuilder.AlterColumn<int>(
                name: "VerifyType",
                schema: "subjects",
                table: "ThirdPartyIdp",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "subjects",
                table: "ThirdPartyIdp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "subjects",
                table: "ThirdPartyIdp",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "subjects",
                table: "ThirdPartyIdp",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ClientSecret",
                schema: "subjects",
                table: "ThirdPartyIdp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "subjects",
                table: "ThirdPartyIdp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThirdPartyIdp",
                schema: "subjects",
                table: "ThirdPartyIdp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyUser_ThirdPartyIdp_ThirdPartyIdpId",
                schema: "subjects",
                table: "ThirdPartyUser",
                column: "ThirdPartyIdpId",
                principalSchema: "subjects",
                principalTable: "ThirdPartyIdp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
