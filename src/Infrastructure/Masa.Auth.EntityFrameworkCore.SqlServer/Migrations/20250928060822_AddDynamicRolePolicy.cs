using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddDynamicRolePolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ControlPolicy",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    DynamicRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlPolicy_DynamicRole_DynamicRoleId",
                        column: x => x.DynamicRoleId,
                        principalSchema: "auth",
                        principalTable: "DynamicRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionIdentifier",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ControlPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionIdentifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionIdentifier_ControlPolicy_ControlPolicyId",
                        column: x => x.ControlPolicyId,
                        principalSchema: "auth",
                        principalTable: "ControlPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceIdentifier",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Service = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ControlPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceIdentifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceIdentifier_ControlPolicy_ControlPolicyId",
                        column: x => x.ControlPolicyId,
                        principalSchema: "auth",
                        principalTable: "ControlPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionIdentifier_ControlPolicyId",
                schema: "auth",
                table: "ActionIdentifier",
                column: "ControlPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPolicy_DynamicRoleId",
                schema: "auth",
                table: "ControlPolicy",
                column: "DynamicRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceIdentifier_ControlPolicyId",
                schema: "auth",
                table: "ResourceIdentifier",
                column: "ControlPolicyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionIdentifier",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ResourceIdentifier",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ControlPolicy",
                schema: "auth");
        }
    }
}
