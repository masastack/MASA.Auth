using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class position2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "organizations",
                table: "Position",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                schema: "organizations",
                table: "Position",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "organizations",
                table: "Position",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTime",
                schema: "organizations",
                table: "Position",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Modifier",
                schema: "organizations",
                table: "Position",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Position_Name",
                schema: "organizations",
                table: "Position",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Position_Name",
                schema: "organizations",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "organizations",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "organizations",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "organizations",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "ModificationTime",
                schema: "organizations",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "organizations",
                table: "Position");
        }
    }
}
