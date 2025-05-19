using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddUserClaimExtend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "auth",
                table: "Role",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Legend",
                schema: "auth",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DynamicRole",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClient",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClient", x => new { x.RoleId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_RoleClient_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicRuleCondition",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogicalOperator = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OperatorType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DynamicRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicRuleCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicRuleCondition_DynamicRole_DynamicRoleId",
                        column: x => x.DynamicRoleId,
                        principalSchema: "auth",
                        principalTable: "DynamicRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicRuleCondition_DynamicRoleId",
                schema: "auth",
                table: "DynamicRuleCondition",
                column: "DynamicRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicRuleCondition",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "RoleClient",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "DynamicRole",
                schema: "auth");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "auth",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Legend",
                schema: "auth",
                table: "Permission");
        }
    }
}
