namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class LDAPIdpEntityTypeConfiguration : IEntityTypeConfiguration<LDAPIdp>
{
    public void Configure(EntityTypeBuilder<LDAPIdp> builder)
    {
        builder.Property(p => p.BaseDn).HasMaxLength(255);
        builder.Property(p => p.UserSearchBaseDn).HasMaxLength(255);
        builder.Property(p => p.GroupSearchBaseDn).HasMaxLength(255);
        builder.Property(p => p.RootUserDn).HasMaxLength(255);
        builder.Property(p => p.RootUserPassword).HasMaxLength(255);
    }
}
