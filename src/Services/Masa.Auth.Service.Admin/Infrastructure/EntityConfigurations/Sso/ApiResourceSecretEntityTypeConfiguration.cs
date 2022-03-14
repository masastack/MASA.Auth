namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceSecretEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceSecret>
{
    public void Configure(EntityTypeBuilder<ApiResourceSecret> builder)
    {
        builder.ToTable(nameof(ApiResourceSecret), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);

        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(250).IsRequired();
    }
}
