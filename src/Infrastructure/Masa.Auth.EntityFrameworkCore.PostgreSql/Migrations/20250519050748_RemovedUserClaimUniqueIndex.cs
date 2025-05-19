// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class RemovedUserClaimUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClaimValue_UserId_Name",
                schema: "auth",
                table: "UserClaimValue");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaimValue_UserId",
                schema: "auth",
                table: "UserClaimValue",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClaimValue_UserId",
                schema: "auth",
                table: "UserClaimValue");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaimValue_UserId_Name",
                schema: "auth",
                table: "UserClaimValue",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }
    }
}
