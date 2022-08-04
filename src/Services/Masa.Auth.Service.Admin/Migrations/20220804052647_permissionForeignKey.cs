using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class permissionForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Role_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Team_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_User_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Role_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId",
                principalSchema: "auth",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Team_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId",
                principalSchema: "auth",
                principalTable: "Team",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_User_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Role_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Team_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_User_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Role_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId",
                principalSchema: "auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Team_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId",
                principalSchema: "auth",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_User_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
