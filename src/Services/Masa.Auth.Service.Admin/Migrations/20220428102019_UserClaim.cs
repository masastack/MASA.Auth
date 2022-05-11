// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UserClaim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaim_ApiScope_ScopeId",
                schema: "sso",
                table: "ApiScopeClaim");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "sso",
                table: "IdentityResourceClaim");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "sso",
                table: "ApiScopeClaim");

            migrationBuilder.DropColumn(
                name: "Scope",
                schema: "sso",
                table: "ApiResourceScope");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "sso",
                table: "ApiResourceClaim");

            migrationBuilder.RenameColumn(
                name: "ScopeId",
                schema: "sso",
                table: "ApiScopeClaim",
                newName: "UserClaimId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiScopeClaim_ScopeId",
                schema: "sso",
                table: "ApiScopeClaim",
                newName: "IX_ApiScopeClaim_UserClaimId");

            migrationBuilder.AddColumn<int>(
                name: "UserClaimId",
                schema: "sso",
                table: "IdentityResourceClaim",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApiScopeId",
                schema: "sso",
                table: "ApiScopeClaim",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "sso",
                table: "ApiScope",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Creator",
                schema: "sso",
                table: "ApiScope",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "sso",
                table: "ApiScope",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTime",
                schema: "sso",
                table: "ApiScope",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Modifier",
                schema: "sso",
                table: "ApiScope",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ApiScopeId",
                schema: "sso",
                table: "ApiResourceScope",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserClaimId",
                schema: "sso",
                table: "ApiResourceClaim",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserClaim",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserClaimType = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaim_UserClaimId",
                schema: "sso",
                table: "IdentityResourceClaim",
                column: "UserClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaim_ApiScopeId",
                schema: "sso",
                table: "ApiScopeClaim",
                column: "ApiScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScope_ApiScopeId",
                schema: "sso",
                table: "ApiResourceScope",
                column: "ApiScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceClaim_UserClaimId",
                schema: "sso",
                table: "ApiResourceClaim",
                column: "UserClaimId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceClaim_UserClaim_UserClaimId",
                schema: "sso",
                table: "ApiResourceClaim",
                column: "UserClaimId",
                principalSchema: "sso",
                principalTable: "UserClaim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceScope_ApiScope_ApiScopeId",
                schema: "sso",
                table: "ApiResourceScope",
                column: "ApiScopeId",
                principalSchema: "sso",
                principalTable: "ApiScope",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaim_ApiScope_ApiScopeId",
                schema: "sso",
                table: "ApiScopeClaim",
                column: "ApiScopeId",
                principalSchema: "sso",
                principalTable: "ApiScope",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaim_UserClaim_UserClaimId",
                schema: "sso",
                table: "ApiScopeClaim",
                column: "UserClaimId",
                principalSchema: "sso",
                principalTable: "UserClaim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityResourceClaim_UserClaim_UserClaimId",
                schema: "sso",
                table: "IdentityResourceClaim",
                column: "UserClaimId",
                principalSchema: "sso",
                principalTable: "UserClaim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceClaim_UserClaim_UserClaimId",
                schema: "sso",
                table: "ApiResourceClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceScope_ApiScope_ApiScopeId",
                schema: "sso",
                table: "ApiResourceScope");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaim_ApiScope_ApiScopeId",
                schema: "sso",
                table: "ApiScopeClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaim_UserClaim_UserClaimId",
                schema: "sso",
                table: "ApiScopeClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityResourceClaim_UserClaim_UserClaimId",
                schema: "sso",
                table: "IdentityResourceClaim");

            migrationBuilder.DropTable(
                name: "UserClaim",
                schema: "sso");

            migrationBuilder.DropIndex(
                name: "IX_IdentityResourceClaim_UserClaimId",
                schema: "sso",
                table: "IdentityResourceClaim");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaim_ApiScopeId",
                schema: "sso",
                table: "ApiScopeClaim");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceScope_ApiScopeId",
                schema: "sso",
                table: "ApiResourceScope");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceClaim_UserClaimId",
                schema: "sso",
                table: "ApiResourceClaim");

            migrationBuilder.DropColumn(
                name: "UserClaimId",
                schema: "sso",
                table: "IdentityResourceClaim");

            migrationBuilder.DropColumn(
                name: "ApiScopeId",
                schema: "sso",
                table: "ApiScopeClaim");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "ModificationTime",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "ApiScopeId",
                schema: "sso",
                table: "ApiResourceScope");

            migrationBuilder.DropColumn(
                name: "UserClaimId",
                schema: "sso",
                table: "ApiResourceClaim");

            migrationBuilder.RenameColumn(
                name: "UserClaimId",
                schema: "sso",
                table: "ApiScopeClaim",
                newName: "ScopeId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiScopeClaim_UserClaimId",
                schema: "sso",
                table: "ApiScopeClaim",
                newName: "IX_ApiScopeClaim_ScopeId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "sso",
                table: "IdentityResourceClaim",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "sso",
                table: "ApiScopeClaim",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                schema: "sso",
                table: "ApiResourceScope",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "sso",
                table: "ApiResourceClaim",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaim_ApiScope_ScopeId",
                schema: "sso",
                table: "ApiScopeClaim",
                column: "ScopeId",
                principalSchema: "sso",
                principalTable: "ApiScope",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
