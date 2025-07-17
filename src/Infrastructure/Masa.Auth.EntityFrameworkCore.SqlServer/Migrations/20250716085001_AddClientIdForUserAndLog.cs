using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddClientIdForUserAndLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                schema: "auth",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                schema: "auth",
                table: "OperationLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ClientId",
                schema: "auth",
                table: "User",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLog_ClientId",
                schema: "auth",
                table: "OperationLog",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_ClientId",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_OperationLog_ClientId",
                schema: "auth",
                table: "OperationLog");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "auth",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "auth",
                table: "OperationLog");
        }
    }
}
