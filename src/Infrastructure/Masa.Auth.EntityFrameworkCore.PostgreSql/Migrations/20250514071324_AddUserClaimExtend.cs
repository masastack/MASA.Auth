using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class AddUserClaimExtend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserClaimExtend",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSourceType = table.Column<int>(type: "integer", nullable: false),
                    DataSourceValue = table.Column<string>(type: "text", nullable: false),
                    ItemText = table.Column<string>(type: "text", nullable: false),
                    ItemValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaimExtend", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaimExtend",
                schema: "auth");
        }
    }
}
