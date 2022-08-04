using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class jc : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_SubjectPermissionRelation_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "PermissionRelationType",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "_businessType");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPermissionRelation_RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPermissionRelation_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPermissionRelation_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "UserId");

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

            migrationBuilder.DropIndex(
                name: "IX_SubjectPermissionRelation_RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropIndex(
                name: "IX_SubjectPermissionRelation_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropIndex(
                name: "IX_SubjectPermissionRelation_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.RenameColumn(
                name: "_businessType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "Discriminator");

            migrationBuilder.AddColumn<int>(
                name: "PermissionRelationType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPermissionRelation_SubjectRelationId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "SubjectRelationId");

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
    }
}
