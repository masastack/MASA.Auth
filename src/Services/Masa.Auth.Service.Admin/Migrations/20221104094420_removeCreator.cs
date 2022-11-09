using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class removeCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_Creator",
                schema: "auth",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_Modifier",
                schema: "auth",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_User_Creator",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_User_Modifier",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_Creator",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_Modifier",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Role_Creator",
                schema: "auth",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_Modifier",
                schema: "auth",
                table: "Role");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Staff_Creator",
                schema: "auth",
                table: "Staff",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_Modifier",
                schema: "auth",
                table: "Staff",
                column: "Modifier");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Creator",
                schema: "auth",
                table: "Role",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Modifier",
                schema: "auth",
                table: "Role",
                column: "Modifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_Creator",
                schema: "auth",
                table: "Role",
                column: "Creator",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_Modifier",
                schema: "auth",
                table: "Role",
                column: "Modifier",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_User_Creator",
                schema: "auth",
                table: "Staff",
                column: "Creator",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_User_Modifier",
                schema: "auth",
                table: "Staff",
                column: "Modifier",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
