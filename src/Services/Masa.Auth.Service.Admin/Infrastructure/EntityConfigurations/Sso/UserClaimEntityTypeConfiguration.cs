namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class UserClaimEntityTypeConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable(nameof(ApiResource), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}
