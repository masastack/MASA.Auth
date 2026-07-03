using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddClientConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "_businessType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                schema: "auth",
                table: "IdentityProvider",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ClientConfig",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordRule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PasswordPrompt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PasswordRuleConfig = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientMessageTemplate",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientConfigId = table.Column<int>(type: "integer", nullable: false),
                    ChannelType = table.Column<int>(type: "integer", nullable: false),
                    Scene = table.Column<int>(type: "integer", nullable: false),
                    ChannelCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMessageTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMessageTemplate_ClientConfig_ClientConfigId",
                        column: x => x.ClientConfigId,
                        principalSchema: "auth",
                        principalTable: "ClientConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientConfig_ClientId",
                schema: "auth",
                table: "ClientConfig",
                column: "ClientId",
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageTemplate_ClientConfigId_ChannelType_Scene",
                schema: "auth",
                table: "ClientMessageTemplate",
                columns: new[] { "ClientConfigId", "ChannelType", "Scene" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMessageTemplate",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ClientConfig",
                schema: "auth");

            migrationBuilder.AlterColumn<string>(
                name: "_businessType",
                schema: "auth",
                table: "SubjectPermissionRelation",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(34)",
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                schema: "auth",
                table: "IdentityProvider",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(21)",
                oldMaxLength: 21);
        }
    }
}
