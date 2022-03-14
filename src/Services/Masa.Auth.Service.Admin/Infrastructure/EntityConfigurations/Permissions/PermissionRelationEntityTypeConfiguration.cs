namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Permissions;

public class PermissionRelationEntityTypeConfiguration : IEntityTypeConfiguration<PermissionRelation>
{
    public void Configure(EntityTypeBuilder<PermissionRelation> builder)
    {
        builder.ToTable(nameof(PermissionRelation), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(pi => pi.Id);

    }
}

