// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModels;

public class CacheStaff
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Account { get; set; } = "";

    public string JobNumber { get; set; } = "";

    public bool Enabled { get; set; }

    public StaffTypes StaffType { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string CompanyName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public AddressValueDto Address { get; set; } = new();

    public DateTime CreationTime { get; set; }

    public GenderTypes Gender { get; set; }

    public Guid? CurrentTeamId { get; set; }


    public static implicit operator CacheStaff(Staff staff)
    {
        return new CacheStaff
        {
            Id = staff.Id,
            Name = staff.Name,
            UserId = staff.UserId,
            PhoneNumber = staff.PhoneNumber,
            JobNumber = staff.JobNumber,
            Enabled = staff.Enabled,
            StaffType = staff.StaffType,
            DisplayName = staff.DisplayName,
            Avatar = staff.Avatar,
            IdCard = staff.IdCard,
            CompanyName = staff.CompanyName,
            Address = staff.Address,
            Email = staff.Email,
            CreationTime = staff.CreationTime,
            Gender = staff.Gender,
            CurrentTeamId = staff.CurrentTeamId
        };
    }
}
