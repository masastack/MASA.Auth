namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class RoleRelationEntityTypeConfiguration : IEntityTypeConfiguration<RoleRelation>
{
    public void Configure(EntityTypeBuilder<RoleRelation> builder)
    {
        builder.ToTable(nameof(RoleRelation), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(ri => ri.Id);
    }
}

