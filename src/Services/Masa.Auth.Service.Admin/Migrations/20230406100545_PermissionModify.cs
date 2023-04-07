using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class PermissionModify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRelation_Permission_ChildPermissionId",
                schema: "auth",
                table: "PermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRelation_Permission_ParentPermissionId",
                schema: "auth",
                table: "PermissionRelation");

            migrationBuilder.RenameColumn(
                name: "ParentPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "LeadingPermissionId");

            migrationBuilder.RenameColumn(
                name: "ChildPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "AffiliationPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRelation_ParentPermissionId_ChildPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "IX_PermissionRelation_LeadingPermissionId_AffiliationPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRelation_ChildPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "IX_PermissionRelation_AffiliationPermissionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentId",
                schema: "auth",
                table: "Permission",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ParentId",
                schema: "auth",
                table: "Permission",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Permission_ParentId",
                schema: "auth",
                table: "Permission",
                column: "ParentId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRelation_Permission_AffiliationPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                column: "AffiliationPermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRelation_Permission_LeadingPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                column: "LeadingPermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Permission_ParentId",
                schema: "auth",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRelation_Permission_AffiliationPermissionId",
                schema: "auth",
                table: "PermissionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRelation_Permission_LeadingPermissionId",
                schema: "auth",
                table: "PermissionRelation");

            migrationBuilder.DropIndex(
                name: "IX_Permission_ParentId",
                schema: "auth",
                table: "Permission");

            migrationBuilder.RenameColumn(
                name: "LeadingPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "ParentPermissionId");

            migrationBuilder.RenameColumn(
                name: "AffiliationPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "ChildPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRelation_LeadingPermissionId_AffiliationPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "IX_PermissionRelation_ParentPermissionId_ChildPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRelation_AffiliationPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                newName: "IX_PermissionRelation_ChildPermissionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentId",
                schema: "auth",
                table: "Permission",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRelation_Permission_ChildPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                column: "ChildPermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRelation_Permission_ParentPermissionId",
                schema: "auth",
                table: "PermissionRelation",
                column: "ParentPermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id");
        }
    }
}
