namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ApiScopeClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeClaim>
{
    public void Configure(EntityTypeBuilder<ApiScopeClaim> builder)
    {
        builder.ToTable(nameof(ApiScopeClaim)).HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(200).IsRequired();
    }
}
