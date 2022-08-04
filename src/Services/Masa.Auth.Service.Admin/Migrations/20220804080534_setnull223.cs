using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class setnull223 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Role_RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Team_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_User_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Role_RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "RoleId",
                principalSchema: "auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Team_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "TeamId",
                principalSchema: "auth",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_User_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Role_RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Team_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_User_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Role_RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "RoleId",
                principalSchema: "auth",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Team_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "TeamId",
                principalSchema: "auth",
                principalTable: "Team",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_User_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
