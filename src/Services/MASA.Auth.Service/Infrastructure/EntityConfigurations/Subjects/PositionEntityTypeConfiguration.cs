namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class PositionEntityTypeConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable(nameof(Position), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(p => p.Id);
        }
    }
}
