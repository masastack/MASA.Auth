// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class StaffCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_UserId",
                schema: "subjects",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                schema: "subjects",
                table: "Staff",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_UserId",
                schema: "subjects",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                schema: "subjects",
                table: "Staff",
                column: "UserId");
        }
    }
}
