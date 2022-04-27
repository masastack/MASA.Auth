using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UpdatePermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PermissionId",
                schema: "permissions",
                table: "PermissionRelation",
                newName: "ParentPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRelation_ParentPermissionId_ChildPermissionId",
                schema: "permissions",
                table: "PermissionRelation",
                columns: new[] { "ParentPermissionId", "ChildPermissionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRelation_Permission_ParentPermissionId",
                schema: "permissions",
                table: "PermissionRelation",
                column: "ParentPermissionId",
                principalSchema: "permissions",
                principalTable: "Permission",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRelation_Permission_ParentPermissionId",
                schema: "permissions",
                table: "PermissionRelation");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRelation_ParentPermissionId_ChildPermissionId",
                schema: "permissions",
                table: "PermissionRelation");

            migrationBuilder.RenameColumn(
                name: "ParentPermissionId",
                schema: "permissions",
                table: "PermissionRelation",
                newName: "PermissionId");
        }
    }
}
