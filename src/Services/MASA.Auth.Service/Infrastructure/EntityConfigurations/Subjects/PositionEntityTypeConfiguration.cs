namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class PositionEntityTypeConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions", AuthDbContext.PERMISSION_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
