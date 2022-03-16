using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
using Masa.Auth.Service.Admin.Infrastructure;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class TeamRoleEntityTypeConfiguration : IEntityTypeConfiguration<TeamRole>
{
    public void Configure(EntityTypeBuilder<TeamRole> builder)
    {
        builder.ToTable(nameof(TeamRole), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(t => t.Id);
    }
}

