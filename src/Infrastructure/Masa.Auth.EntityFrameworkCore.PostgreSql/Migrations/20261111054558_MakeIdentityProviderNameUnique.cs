using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class MakeIdentityProviderNameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 幂等删除:存量库可能因备份恢复/手工清理导致索引缺失,
            // 若用 DropIndex 会在索引不存在时直接报 42704,故改用 DROP INDEX IF EXISTS(与 RestoreUniqueIndexes 同风格)。
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS auth."IX_IdentityProvider_Name";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_Name",
                schema: "auth",
                table: "IdentityProvider",
                column: "Name",
                unique: true,
                filter: "NOT \"IsDeleted\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS auth."IX_IdentityProvider_Name";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvider_Name",
                schema: "auth",
                table: "IdentityProvider",
                column: "Name",
                filter: "NOT \"IsDeleted\"");
        }
    }
}
