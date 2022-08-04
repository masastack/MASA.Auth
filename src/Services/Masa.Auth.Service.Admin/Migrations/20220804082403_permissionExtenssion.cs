using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class permissionExtenssion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermission_Permission_PermissionId",
                schema: "auth",
                table: "UserPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermission_User_UserId",
                schema: "auth",
                table: "UserPermission");

            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "TeamPermission",
                schema: "auth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermission",
                schema: "auth",
                table: "UserPermission");

            migrationBuilder.RenameTable(
                name: "UserPermission",
                schema: "auth",
                newName: "SubjectPermissionRelation",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermission_UserId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "IX_SubjectPermissionRelation_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermission_PermissionId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                newName: "IX_SubjectPermissionRelation_PermissionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AddColumn<int>(
                name: "TeamMemberType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "_businessType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectPermissionRelation",
                schema: "auth",
                table: "SubjectPermissionRelation",
                column: "Id");

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
                name: "FK_SubjectPermissionRelation_Permission_PermissionId",
                schema: "auth",
                table: "SubjectPermissionRelation");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectPermissionRelation",
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

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "TeamId",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "TeamMemberType",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.DropColumn(
                name: "_businessType",
                schema: "auth",
                table: "SubjectPermissionRelation");

            migrationBuilder.RenameTable(
                name: "SubjectPermissionRelation",
                schema: "auth",
                newName: "UserPermission",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectPermissionRelation_UserId",
                schema: "auth",
                table: "UserPermission",
                newName: "IX_UserPermission_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectPermissionRelation_PermissionId",
                schema: "auth",
                table: "UserPermission",
                newName: "IX_UserPermission_PermissionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "auth",
                table: "UserPermission",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermission",
                schema: "auth",
                table: "UserPermission",
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
                name: "TeamPermission",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Effect = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "auth",
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPermission_Team_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "auth",
                        principalTable: "Team",
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
                name: "IX_TeamPermission_PermissionId",
                schema: "auth",
                table: "TeamPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermission_TeamId",
                schema: "auth",
                table: "TeamPermission",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermission_Permission_PermissionId",
                schema: "auth",
                table: "UserPermission",
                column: "PermissionId",
                principalSchema: "auth",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermission_User_UserId",
                schema: "auth",
                table: "UserPermission",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
