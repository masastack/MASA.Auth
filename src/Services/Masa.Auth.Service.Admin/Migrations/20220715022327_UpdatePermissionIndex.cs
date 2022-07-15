// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UpdatePermissionIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_Permission_AppId_Code",
                schema: "auth",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "MemberCount",
                schema: "auth",
                table: "Team");

            migrationBuilder.AlterColumn<string>(
                name: "SystemId",
                schema: "auth",
                table: "Permission",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OperationDescription",
                schema: "auth",
                table: "OperationLog",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_SystemId_AppId_Code",
                schema: "auth",
                table: "Permission",
                columns: new[] { "SystemId", "AppId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permission_SystemId_AppId_Code",
                schema: "auth",
                table: "Permission");

            migrationBuilder.AddColumn<int>(
                name: "MemberCount",
                schema: "auth",
                table: "Team",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SystemId",
                schema: "auth",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "OperationDescription",
                schema: "auth",
                table: "OperationLog",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_AppId_Code",
                schema: "auth",
                table: "Permission",
                columns: new[] { "AppId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
