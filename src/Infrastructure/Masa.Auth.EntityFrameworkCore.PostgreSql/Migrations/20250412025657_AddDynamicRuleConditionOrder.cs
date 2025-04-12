using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class AddDynamicRuleConditionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "auth",
                table: "DynamicRuleCondition",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                schema: "auth",
                table: "DynamicRuleCondition");
        }
    }
}
