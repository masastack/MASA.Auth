namespace Masa.Auth.Service.Infrastructure.EntityConfigurations;

public class PermissionItemEntityTypeConfiguration : IEntityTypeConfiguration<PermissionItem>
{
    public void Configure(EntityTypeBuilder<PermissionItem> builder)
    {
        builder.ToTable(nameof(PermissionItem), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(pi => pi.Id);


    }
}

