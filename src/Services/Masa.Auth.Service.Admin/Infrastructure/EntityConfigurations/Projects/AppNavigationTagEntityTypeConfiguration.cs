namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Projects;

public class AppNavigationTagEntityTypeConfiguration : IEntityTypeConfiguration<AppNavigationTag>
{
    public void Configure(EntityTypeBuilder<AppNavigationTag> builder)
    {
        builder.ToTable(nameof(AppNavigationTag), AuthDbContext.PROJECTS_SCHEMA);
        builder.HasKey(ri => ri.Id);
        builder.Property(p => p.AppIdentity).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Tag).HasMaxLength(255);
    }
}
