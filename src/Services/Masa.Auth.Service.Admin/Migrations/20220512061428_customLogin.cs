using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class customLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomLogin",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomLogin_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "sso",
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomLogin_User_Creator",
                        column: x => x.Creator,
                        principalSchema: "subjects",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomLogin_User_Modifier",
                        column: x => x.Modifier,
                        principalSchema: "subjects",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomLoginThirdPartyIdp",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThirdPartyIdpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomLoginId = table.Column<int>(type: "int", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomLoginThirdPartyIdp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomLoginThirdPartyIdp_CustomLogin_CustomLoginId",
                        column: x => x.CustomLoginId,
                        principalSchema: "sso",
                        principalTable: "CustomLogin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomLoginThirdPartyIdp_IdentityProvider_ThirdPartyIdpId",
                        column: x => x.ThirdPartyIdpId,
                        principalSchema: "subjects",
                        principalTable: "IdentityProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegisterField",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterFieldType = table.Column<int>(type: "int", nullable: false),
                    CustomLoginId = table.Column<int>(type: "int", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisterField_CustomLogin_CustomLoginId",
                        column: x => x.CustomLoginId,
                        principalSchema: "sso",
                        principalTable: "CustomLogin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_ClientId",
                schema: "sso",
                table: "CustomLogin",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_Creator",
                schema: "sso",
                table: "CustomLogin",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_Modifier",
                schema: "sso",
                table: "CustomLogin",
                column: "Modifier");

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_Name",
                schema: "sso",
                table: "CustomLogin",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CustomLoginThirdPartyIdp_CustomLoginId",
                schema: "sso",
                table: "CustomLoginThirdPartyIdp",
                column: "CustomLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomLoginThirdPartyIdp_ThirdPartyIdpId",
                schema: "sso",
                table: "CustomLoginThirdPartyIdp",
                column: "ThirdPartyIdpId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterField_CustomLoginId",
                schema: "sso",
                table: "RegisterField",
                column: "CustomLoginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomLoginThirdPartyIdp",
                schema: "sso");

            migrationBuilder.DropTable(
                name: "RegisterField",
                schema: "sso");

            migrationBuilder.DropTable(
                name: "CustomLogin",
                schema: "sso");
        }
    }
}
