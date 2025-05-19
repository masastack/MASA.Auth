using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class RemovedUserClaimUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClaimValue_UserId_Name",
                schema: "auth",
                table: "UserClaimValue");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "auth",
                table: "UserClaimValue",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaimValue_UserId",
                schema: "auth",
                table: "UserClaimValue",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClaimValue_UserId",
                schema: "auth",
                table: "UserClaimValue");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "auth",
                table: "UserClaimValue",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaimValue_UserId_Name",
                schema: "auth",
                table: "UserClaimValue",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }
    }
}
