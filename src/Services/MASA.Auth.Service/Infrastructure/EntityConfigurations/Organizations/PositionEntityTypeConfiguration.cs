namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Organizations;

public class PositionEntityTypeConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable(nameof(Position), AuthDbContext.ORGANIZATION_SCHEMA);
        builder.HasKey(p => p.Id);

        builder.Property(d => d.Name).HasMaxLength(20);
    }
}

