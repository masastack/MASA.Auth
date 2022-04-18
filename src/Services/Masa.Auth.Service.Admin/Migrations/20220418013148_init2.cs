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

            migrationBuilder.AddColumn<int>(
                name: "QuantityAvailable",
                schema: "permissions",
                table: "Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "FK_TeamRole_Role_RoleId",
                schema: "subjects",
                table: "TeamRole");

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
                name: "QuantityAvailable",
                schema: "permissions",
                table: "Role");
        }
    }
}
