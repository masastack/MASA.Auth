using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class AddStaffTeamId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentTeamId",
                schema: "auth",
                table: "Staff",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_CurrentTeamId",
                schema: "auth",
                table: "Staff",
                column: "CurrentTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Team_CurrentTeamId",
                schema: "auth",
                table: "Staff",
                column: "CurrentTeamId",
                principalSchema: "auth",
                principalTable: "Team",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Team_CurrentTeamId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_CurrentTeamId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "CurrentTeamId",
                schema: "auth",
                table: "Staff");
        }
    }
}
