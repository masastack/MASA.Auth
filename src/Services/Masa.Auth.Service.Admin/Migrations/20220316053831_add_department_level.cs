using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.Service.Admin.Migrations
{
    public partial class add_department_level : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsumedTime",
                schema: "sso",
                table: "PersistedGrant",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "sso",
                table: "IdentityResource",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Emphasize",
                schema: "sso",
                table: "IdentityResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "sso",
                table: "IdentityResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "sso",
                table: "IdentityResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Required",
                schema: "sso",
                table: "IdentityResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInDiscoveryDocument",
                schema: "sso",
                table: "IdentityResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "sso",
                table: "IdentityResource",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                schema: "organizations",
                table: "Department",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "sso",
                table: "ClientSecret",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                schema: "sso",
                table: "ClientSecret",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostLogoutRedirectUri",
                schema: "sso",
                table: "ClientPostLogoutRedirectUri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AbsoluteRefreshTokenLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccessTokenLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccessTokenType",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AllowAccessTokensViaBrowser",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowOfflineAccess",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPlainTextPkce",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowRememberConsent",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AlwaysIncludeUserClaimsInIdToken",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AlwaysSendClientClaims",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AuthorizationCodeLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "BackChannelLogoutSessionRequired",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ConsentLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "sso",
                table: "Client",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeviceCodeLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableLocalLogin",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FrontChannelLogoutSessionRequired",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdentityTokenLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeJwtId",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                schema: "sso",
                table: "Client",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenExpiration",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenUsage",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequireClientSecret",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireConsent",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequirePkce",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireRequestObject",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SlidingRefreshTokenLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "UpdateAccessTokenClaimsOnRefresh",
                schema: "sso",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "sso",
                table: "Client",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSsoLifetime",
                schema: "sso",
                table: "Client",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Emphasize",
                schema: "sso",
                table: "ApiScope",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "sso",
                table: "ApiScope",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Required",
                schema: "sso",
                table: "ApiScope",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInDiscoveryDocument",
                schema: "sso",
                table: "ApiScope",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "sso",
                table: "ApiResourceSecret",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                schema: "sso",
                table: "ApiResourceSecret",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "sso",
                table: "ApiResource",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "sso",
                table: "ApiResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                schema: "sso",
                table: "ApiResource",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "sso",
                table: "ApiResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInDiscoveryDocument",
                schema: "sso",
                table: "ApiResource",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "sso",
                table: "ApiResource",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumedTime",
                schema: "sso",
                table: "PersistedGrant");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "Emphasize",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "Required",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "ShowInDiscoveryDocument",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "sso",
                table: "IdentityResource");

            migrationBuilder.DropColumn(
                name: "Level",
                schema: "organizations",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "sso",
                table: "ClientSecret");

            migrationBuilder.DropColumn(
                name: "Expiration",
                schema: "sso",
                table: "ClientSecret");

            migrationBuilder.DropColumn(
                name: "PostLogoutRedirectUri",
                schema: "sso",
                table: "ClientPostLogoutRedirectUri");

            migrationBuilder.DropColumn(
                name: "AbsoluteRefreshTokenLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AccessTokenLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AccessTokenType",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AllowAccessTokensViaBrowser",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AllowOfflineAccess",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AllowPlainTextPkce",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AllowRememberConsent",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AlwaysIncludeUserClaimsInIdToken",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AlwaysSendClientClaims",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AuthorizationCodeLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "BackChannelLogoutSessionRequired",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "ConsentLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "DeviceCodeLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "EnableLocalLogin",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "FrontChannelLogoutSessionRequired",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "IdentityTokenLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "IncludeJwtId",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiration",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "RefreshTokenUsage",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "RequireClientSecret",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "RequireConsent",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "RequirePkce",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "RequireRequestObject",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "SlidingRefreshTokenLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "UpdateAccessTokenClaimsOnRefresh",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "UserSsoLifetime",
                schema: "sso",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Emphasize",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "Required",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "ShowInDiscoveryDocument",
                schema: "sso",
                table: "ApiScope");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "sso",
                table: "ApiResourceSecret");

            migrationBuilder.DropColumn(
                name: "Expiration",
                schema: "sso",
                table: "ApiResourceSecret");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "sso",
                table: "ApiResource");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "sso",
                table: "ApiResource");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                schema: "sso",
                table: "ApiResource");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "sso",
                table: "ApiResource");

            migrationBuilder.DropColumn(
                name: "ShowInDiscoveryDocument",
                schema: "sso",
                table: "ApiResource");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "sso",
                table: "ApiResource");
        }
    }
}
