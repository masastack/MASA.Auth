namespace Masa.Auth.Service.Infrastructure.EntityConfigurations;

public class RoleItemEntityTypeConfiguration : IEntityTypeConfiguration<RoleItem>
{
    public void Configure(EntityTypeBuilder<RoleItem> builder)
    {
        builder.ToTable(nameof(RoleItem), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(ri => ri.Id);
    }
}

