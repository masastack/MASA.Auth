namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceScope>
{
    public void Configure(EntityTypeBuilder<ApiResourceScope> builder)
    {
        builder.ToTable(nameof(ApiResourceScope)).HasKey(x => x.Id);
        builder.Property(x => x.Scope).HasMaxLength(200).IsRequired();
    }
}
