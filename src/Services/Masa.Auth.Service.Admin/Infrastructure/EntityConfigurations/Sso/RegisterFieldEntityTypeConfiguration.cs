namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class RegisterFieldEntityTypeConfiguration : IEntityTypeConfiguration<RegisterField>
{
    public void Configure(EntityTypeBuilder<RegisterField> builder)
    {
        builder.ToTable(nameof(RegisterField), AuthDbContext.SSO_SCHEMA).HasKey(registerField => registerField.Id);
    }
}
