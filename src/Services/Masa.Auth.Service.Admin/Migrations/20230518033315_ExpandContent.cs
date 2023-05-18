using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class ExpandContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpandContent",
                schema: "auth",
                table: "IntegrationEventLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpandContent",
                schema: "auth",
                table: "IntegrationEventLog");
        }
    }
}
