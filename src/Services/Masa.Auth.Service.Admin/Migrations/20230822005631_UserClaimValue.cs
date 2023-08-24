using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class UserClaimValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserClaimValue",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaimValue_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaimValue_UserId_Name",
                schema: "auth",
                table: "UserClaimValue",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaimValue",
                schema: "auth");
        }
    }
}
