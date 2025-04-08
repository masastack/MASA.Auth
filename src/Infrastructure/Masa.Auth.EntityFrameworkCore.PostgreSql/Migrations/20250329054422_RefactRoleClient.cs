using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class RefactRoleClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClient",
                schema: "auth",
                table: "RoleClient");

            migrationBuilder.DropIndex(
                name: "IX_RoleClient_RoleId",
                schema: "auth",
                table: "RoleClient");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "auth",
                table: "RoleClient");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "auth",
                table: "RoleClient",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClient",
                schema: "auth",
                table: "RoleClient",
                columns: new[] { "RoleId", "ClientId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClient",
                schema: "auth",
                table: "RoleClient");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "auth",
                table: "RoleClient",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "auth",
                table: "RoleClient",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClient",
                schema: "auth",
                table: "RoleClient",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClient_RoleId",
                schema: "auth",
                table: "RoleClient",
                column: "RoleId");
        }
    }
}
