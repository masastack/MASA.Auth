// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.JobNumber).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.Property(s => s.JobNumber).HasMaxLength(20);
        builder.HasOne(s => s.User).WithOne(user => user.Staff).HasForeignKey<Staff>(s => s.UserId);
        builder.HasIndex(s => s.UserId).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasOne(s => s.Position).WithMany().HasForeignKey(s => s.PositionId).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasMany(s => s.DepartmentStaffs).WithOne(a => a.Staff).HasForeignKey(ds => ds.StaffId);
        builder.HasMany(s => s.TeamStaffs).WithOne(ts => ts.Staff).HasForeignKey(ts => ts.StaffId);
        builder.OwnsOne(s => s.Address);
    }
}

