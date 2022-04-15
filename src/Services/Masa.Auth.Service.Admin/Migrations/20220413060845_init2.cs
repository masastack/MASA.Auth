using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                schema: "permissions",
                table: "Role",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifierUserId",
                schema: "permissions",
                table: "Role",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.DropIndex(
                name: "IX_Role_CreatorUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_ModifierUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                schema: "permissions",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                schema: "permissions",
                table: "Role");
        }
    }
}
