using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class permission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamPermission_Permission_PermissionId",
                schema: "auth",
                table: "TeamPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamPermission_Team_TeamId",
                schema: "auth",
                table: "TeamPermission");

            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserPermission",
                schema: "auth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamPermission",
                schema: "auth",
                table: "TeamPermission");

            migrationBuilder.RenameTable(
                name: "TeamPermission",
                schema: "auth",
                newName: "SubjectPermissionRelation",
                newSchema: "auth");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "SubjectRelationId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamPermission_TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "IX_SubjectPermissionRelation_SubjectRelationId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamPermission_PermissionId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "IX_SubjectPermissionRelation_PermissionId");

            migrationBuilder.AlterColumn<int>(
                name: "TeamMemberType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PermissionRelationType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectPermissionRelation",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectPermissionRelation_Permission_PermissionId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "PermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectPermissionRelation_Permission_PermissionId",
                schema: "auth",
                table: "SubjectPermissionRelation");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectPermissionRelation",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "PermissionRelationType",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.RenameTable(
                name: "SubjectPermissionRelation",
                schema: "auth",
                newName: "TeamPermission",
                newSchema: "auth");

            migrationBuilder.RenameColumn(
                name: "SubjectRelationId",
                schema: "auth",
                table: "TeamPermission",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectPermissionRelation_SubjectRelationId",
                schema: "auth",
                table: "TeamPermission",
                newName: "IX_TeamPermission_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectPermissionRelation_PermissionId",
                schema: "auth",
                table: "TeamPermission",
                newName: "IX_TeamPermission_PermissionId");

            migrationBuilder.AlterColumn<int>(
                name: "TeamMemberType",
                schema: "auth",
                table: "TeamPermission",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamPermission",
                schema: "auth",
                table: "TeamPermission",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "auth",
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermission",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Effect = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "auth",
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermission_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "auth",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                schema: "auth",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_PermissionId",
                schema: "auth",
                table: "UserPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_UserId",
                schema: "auth",
                table: "UserPermission",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPermission_Permission_PermissionId",
                schema: "auth",
                table: "TeamPermission",
                column: "PermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPermission_Team_TeamId",
                schema: "auth",
                table: "TeamPermission",
                column: "TeamId",
                principalSchema: "auth",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
