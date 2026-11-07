using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class RestoreUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 幂等删除：存量库可能因备份恢复/手工清理导致索引缺失，
            // 若用 DropIndex 会在索引不存在时直接报 42704，故改用 DROP INDEX IF EXISTS。
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS auth."IX_UserSystemBusinessData_UserId_SystemId";
                DROP INDEX IF EXISTS auth."IX_User_Account";
                DROP INDEX IF EXISTS auth."IX_User_Email";
                DROP INDEX IF EXISTS auth."IX_User_IdCard";
                DROP INDEX IF EXISTS auth."IX_User_PhoneNumber";
                DROP INDEX IF EXISTS auth."IX_ThirdPartyUser_ThirdPartyIdpId";
                DROP INDEX IF EXISTS auth."IX_Team_Name";
                DROP INDEX IF EXISTS auth."IX_Staff_JobNumber";
                DROP INDEX IF EXISTS auth."IX_Staff_UserId";
                DROP INDEX IF EXISTS auth."IX_Position_Name";
                DROP INDEX IF EXISTS auth."IX_PermissionRelation_LeadingPermissionId_AffiliationPermissio~";
                DROP INDEX IF EXISTS auth."IX_Permission_SystemId_AppId_Code";
                DROP INDEX IF EXISTS auth."IX_IdentityProvider_Name";
                DROP INDEX IF EXISTS auth."IX_Department_Level";
                DROP INDEX IF EXISTS auth."IX_Department_Name_ParentId";
                DROP INDEX IF EXISTS auth."IX_CustomLogin_Name";
                """);

            migrationBuilder.AlterColumn<string>(
                name: "ThridPartyIdentity",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_UserSystemBusinessData_UserId_SystemId",
                schema: "auth",
                table: "UserSystemBusinessData",
                columns: new[] { "UserId", "SystemId" },
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_User_Account",
                schema: "auth",
                table: "User",
                column: "Account",
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "auth",
                table: "User",
                column: "Email",
                unique: true,
                filter: "NOT \"IsDeleted\" and \"Email\"!=''");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdCard",
                schema: "auth",
                table: "User",
                column: "IdCard",
                unique: true,
                filter: "NOT \"IsDeleted\" and \"IdCard\"!=''");

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNumber",
                schema: "auth",
                table: "User",
                column: "PhoneNumber",
                unique: true,
                filter: "NOT \"IsDeleted\" and \"PhoneNumber\"!=''");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyUser_ThirdPartyIdpId_ThridPartyIdentity",
                schema: "auth",
                table: "ThirdPartyUser",
                columns: new[] { "ThirdPartyIdpId", "ThridPartyIdentity" },
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_Team_Name",
                schema: "auth",
                table: "Team",
                column: "Name",
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_JobNumber",
                schema: "auth",
                table: "Staff",
                column: "JobNumber",
                unique: true,
                filter: "NOT \"IsDeleted\" and \"JobNumber\"!=''");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff",
                column: "UserId",
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position",
                column: "Name",
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRelation_LeadingPermissionId_AffiliationPermissio~",
                schema: "auth",
                table: "PermissionRelation",
                columns: new[] { "LeadingPermissionId", "AffiliationPermissionId" },
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_SystemId_AppId_Code",
                schema: "auth",
                table: "Permission",
                columns: new[] { "SystemId", "AppId", "Code" },
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_Name",
                schema: "auth",
                table: "IdentityProvider",
                column: "Name",
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Level",
                schema: "auth",
                table: "Department",
                column: "Level",
                unique: true,
                filter: "\"Level\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name_ParentId",
                schema: "auth",
                table: "Department",
                columns: new[] { "Name", "ParentId" },
                unique: true,
                filter: "NOT \"IsDeleted\"");

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_Name",
                schema: "auth",
                table: "CustomLogin",
                column: "Name",
                unique: true,
                filter: "NOT \"IsDeleted\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSystemBusinessData_UserId_SystemId",
                schema: "auth",
                table: "UserSystemBusinessData");

            migrationBuilder.DropIndex(
                name: "IX_User_Account",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IdCard",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_PhoneNumber",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyUser_ThirdPartyIdpId_ThridPartyIdentity",
                schema: "auth",
                table: "ThirdPartyUser");

            migrationBuilder.DropIndex(
                name: "IX_Team_Name",
                schema: "auth",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Staff_JobNumber",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRelation_LeadingPermissionId_AffiliationPermissio~",
                schema: "auth",
                table: "PermissionRelation");

            migrationBuilder.DropIndex(
                name: "IX_Permission_SystemId_AppId_Code",
                schema: "auth",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_IdentityProvider_Name",
                schema: "auth",
                table: "IdentityProvider");

            migrationBuilder.DropIndex(
                name: "IX_Department_Level",
                schema: "auth",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_Name_ParentId",
                schema: "auth",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_CustomLogin_Name",
                schema: "auth",
                table: "CustomLogin");

            migrationBuilder.AlterColumn<string>(
                name: "ThridPartyIdentity",
                schema: "auth",
                table: "ThirdPartyUser",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_UserSystemBusinessData_UserId_SystemId",
                schema: "auth",
                table: "UserSystemBusinessData",
                columns: new[] { "UserId", "SystemId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_Account",
                schema: "auth",
                table: "User",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "auth",
                table: "User",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdCard",
                schema: "auth",
                table: "User",
                column: "IdCard");

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNumber",
                schema: "auth",
                table: "User",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyUser_ThirdPartyIdpId",
                schema: "auth",
                table: "ThirdPartyUser",
                column: "ThirdPartyIdpId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_Name",
                schema: "auth",
                table: "Team",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_JobNumber",
                schema: "auth",
                table: "Staff",
                column: "JobNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                schema: "auth",
                table: "Staff",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Position_Name",
                schema: "auth",
                table: "Position",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRelation_LeadingPermissionId_AffiliationPermissio~",
                schema: "auth",
                table: "PermissionRelation",
                columns: new[] { "LeadingPermissionId", "AffiliationPermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_SystemId_AppId_Code",
                schema: "auth",
                table: "Permission",
                columns: new[] { "SystemId", "AppId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_Name",
                schema: "auth",
                table: "IdentityProvider",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Level",
                schema: "auth",
                table: "Department",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name_ParentId",
                schema: "auth",
                table: "Department",
                columns: new[] { "Name", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomLogin_Name",
                schema: "auth",
                table: "CustomLogin",
                column: "Name");
        }
    }
}
