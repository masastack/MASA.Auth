using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class RemovePersistedGrantField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "auth",
                table: "PersistedGrant");

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
                name: "CreationTime",
                schema: "auth",
                table: "DeviceFlowCodes");

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
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "auth",
                table: "PersistedGrant",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                schema: "auth",
                table: "PersistedGrant",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "auth",
                table: "PersistedGrant",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "auth",
                table: "PersistedGrant",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTime",
                schema: "auth",
                table: "PersistedGrant",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Modifier",
                schema: "auth",
                table: "PersistedGrant",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTime",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Modifier",
                schema: "auth",
                table: "DeviceFlowCodes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
