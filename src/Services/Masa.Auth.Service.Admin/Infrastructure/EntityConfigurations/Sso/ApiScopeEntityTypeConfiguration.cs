namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiScope>
{
    public void Configure(EntityTypeBuilder<ApiScope> builder)
    {
        builder.ToTable(nameof(ApiScope), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique().HasFilter(null);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasMany(x => x.UserClaims).WithOne(x => x.Scope).HasForeignKey(x => x.ScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
