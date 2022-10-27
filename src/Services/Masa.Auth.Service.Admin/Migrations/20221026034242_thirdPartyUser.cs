using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class thirdPartyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_User_Creator",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyUser_User_Modifier",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyUser_Creator",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyUser_Modifier",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.AlterColumn<Guid>(
                name: "Modifier",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Creator",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Modifier",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Creator",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyUser_Creator",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyUser_Modifier",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "Modifier");

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
        }
    }
}
