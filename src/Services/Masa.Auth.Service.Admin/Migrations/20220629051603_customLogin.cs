using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class customLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomLogin_Client_ClientId",
                schema: "auth",
                table: "CustomLogin");

            migrationBuilder.DropIndex(
                name: "IX_CustomLogin_ClientId",
                schema: "auth",
                table: "CustomLogin");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "auth",
                table: "CustomLogin",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                schema: "auth",
                table: "CustomLogin",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_ClientId",
                schema: "auth",
                table: "CustomLogin",
                column: "ClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomLogin_Client_ClientId",
                schema: "auth",
                table: "CustomLogin",
                column: "ClientId",
                principalSchema: "auth",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
