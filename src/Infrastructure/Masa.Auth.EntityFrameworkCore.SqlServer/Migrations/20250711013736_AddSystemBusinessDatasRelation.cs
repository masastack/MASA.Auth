using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddSystemBusinessDatasRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WebhookEvent",
                schema: "auth",
                table: "Webhook",
                newName: "Event");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSystemBusinessData_User_UserId",
                schema: "auth",
                table: "UserSystemBusinessData",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSystemBusinessData_User_UserId",
                schema: "auth",
                table: "UserSystemBusinessData");

            migrationBuilder.RenameColumn(
                name: "Event",
                schema: "auth",
                table: "Webhook",
                newName: "WebhookEvent");
        }
    }
}
