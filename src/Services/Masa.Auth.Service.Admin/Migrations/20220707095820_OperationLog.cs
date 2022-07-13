using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class OperationLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.CreateTable(
                name: "OperationLog",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    OperationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff",
                column: "PositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationLog",
                schema: "auth");

            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                schema: "auth",
                table: "Staff",
                column: "PositionId",
                unique: true,
                filter: "[PositionId] IS NOT NULL");
        }
    }
}
