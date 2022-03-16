using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
using Masa.Auth.Service.Admin.Infrastructure;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyPlatformEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<ThirdPartyIdp> builder)
    {
        builder.ToTable(nameof(ThirdPartyIdp), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(p => p.Id);
    }
}

