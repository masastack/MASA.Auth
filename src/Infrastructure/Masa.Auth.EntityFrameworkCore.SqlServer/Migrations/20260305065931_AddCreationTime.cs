using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddCreationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "auth",
                table: "PersistedGrant");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "auth",
                table: "PersistedGrant");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "auth",
                table: "PersistedGrant");

            migrationBuilder.DropColumn(
                name: "ModificationTime",
                schema: "auth",
                table: "PersistedGrant");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "auth",
                table: "PersistedGrant");

            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "auth",
                table: "DeviceFlowCodes");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "auth",
                table: "DeviceFlowCodes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "auth",
                table: "DeviceFlowCodes");

            migrationBuilder.DropColumn(
                name: "ModificationTime",
                schema: "auth",
                table: "DeviceFlowCodes");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "auth",
                table: "DeviceFlowCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                schema: "auth",
                table: "PersistedGrant",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "auth",
                table: "PersistedGrant",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "auth",
                table: "PersistedGrant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTime",
                schema: "auth",
                table: "PersistedGrant",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Modifier",
                schema: "auth",
                table: "PersistedGrant",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTime",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Modifier",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
