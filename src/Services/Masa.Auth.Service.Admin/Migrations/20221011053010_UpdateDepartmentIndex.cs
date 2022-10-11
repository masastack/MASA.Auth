using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UpdateDepartmentIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Department_Name",
                schema: "auth",
                table: "Department");

            migrationBuilder.RenameIndex(
                name: "index_state_timessent_modificationtime",
                schema: "auth",
                table: "IntegrationEventLog",
                newName: "IX_State_TimesSent_MTime");

            migrationBuilder.RenameIndex(
                name: "index_state_modificationtime",
                schema: "auth",
                table: "IntegrationEventLog",
                newName: "IX_State_MTime");

            migrationBuilder.RenameIndex(
                name: "index_eventid_version",
                schema: "auth",
                table: "IntegrationEventLog",
                newName: "IX_EventId_Version");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name_ParentId",
                schema: "auth",
                table: "Department",
                columns: new[] { "Name", "ParentId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Department_Name_ParentId",
                schema: "auth",
                table: "Department");

            migrationBuilder.RenameIndex(
                name: "IX_State_TimesSent_MTime",
                schema: "auth",
                table: "IntegrationEventLog",
                newName: "index_state_timessent_modificationtime");

            migrationBuilder.RenameIndex(
                name: "IX_State_MTime",
                schema: "auth",
                table: "IntegrationEventLog",
                newName: "index_state_modificationtime");

            migrationBuilder.RenameIndex(
                name: "IX_EventId_Version",
                schema: "auth",
                table: "IntegrationEventLog",
                newName: "index_eventid_version");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name",
                schema: "auth",
                table: "Department",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
