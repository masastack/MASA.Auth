using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class init8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "QuantityAvailable",
                schema: "permissions",
                table: "Role",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Limit",
                schema: "permissions",
                table: "Role",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRole_RoleId",
                schema: "subjects",
                table: "TeamRole",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamRole_Role_RoleId",
                schema: "subjects",
                table: "TeamRole",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamRole_Role_RoleId",
                schema: "subjects",
                table: "TeamRole");

            migrationBuilder.DropIndex(
                name: "IX_TeamRole_RoleId",
                schema: "subjects",
                table: "TeamRole");

            migrationBuilder.AlterColumn<long>(
                name: "QuantityAvailable",
                schema: "permissions",
                table: "Role",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Limit",
                schema: "permissions",
                table: "Role",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
