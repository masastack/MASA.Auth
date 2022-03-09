namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceClaim>
{
    public void Configure(EntityTypeBuilder<ApiResourceClaim> builder)
    {
        builder.ToTable(nameof(ApiResourceClaim)).HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(200).IsRequired();
    }
}
