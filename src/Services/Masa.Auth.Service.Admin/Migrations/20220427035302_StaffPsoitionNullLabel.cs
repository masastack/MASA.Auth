// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class StaffPsoitionNullLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                schema: "subjects",
                table: "Staff");

            migrationBuilder.AlterColumn<Guid>(
                name: "PositionId",
                schema: "subjects",
                table: "Staff",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                schema: "subjects",
                table: "Staff",
                column: "PositionId",
                unique: true,
                filter: "[PositionId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                schema: "subjects",
                table: "Staff");

            migrationBuilder.AlterColumn<Guid>(
                name: "PositionId",
                schema: "subjects",
                table: "Staff",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                schema: "subjects",
                table: "Staff",
                column: "PositionId",
                unique: true);
        }
    }
}
