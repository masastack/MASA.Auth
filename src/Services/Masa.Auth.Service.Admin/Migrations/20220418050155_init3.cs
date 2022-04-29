// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_ParentId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_ParentId",
                schema: "permissions",
                table: "RoleRelation",
                column: "ParentId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_ParentId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_ParentId",
                schema: "permissions",
                table: "RoleRelation",
                column: "ParentId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRelation_Role_RoleId",
                schema: "permissions",
                table: "RoleRelation",
                column: "RoleId",
                principalSchema: "permissions",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
