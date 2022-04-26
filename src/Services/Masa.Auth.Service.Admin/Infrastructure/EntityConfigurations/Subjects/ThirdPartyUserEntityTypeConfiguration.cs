namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyUserEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyUser>
{
    public void Configure(EntityTypeBuilder<ThirdPartyUser> builder)
    {
        builder.ToTable(nameof(ThirdPartyUser), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(tpu => tpu.Id);
        builder.HasOne(tpu => tpu.User).WithMany().HasForeignKey(tpu => tpu.UserId);
        builder.HasOne(tpu => tpu.ThirdPartyIdp).WithMany().HasForeignKey(tpu => tpu.ThirdPartyIdpId);
        builder.HasOne(tpu => tpu.CreateUser).WithMany().HasForeignKey(tpu => tpu.Creator);
        builder.HasOne(tpu => tpu.ModifyUser).WithMany().HasForeignKey(tpu => tpu.Modifier);
        builder.HasOne(tpu => tpu.ThirdPartyIdp).WithMany().HasForeignKey(tpu => tpu.ThirdPartyIdpId);
    }
}

