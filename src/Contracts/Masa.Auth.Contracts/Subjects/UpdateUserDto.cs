// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserDto
{
    public Guid Id { get; set; }

    public string Account { get; set; }

    public string? Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string? IdCard { get; set; }

    public string? CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    public string? Landline { get; set; }

    public string? Email { get; set; }

    public AddressValueDto Address { get; set; }

    public string? Department { get; set; }

    public string? Position { get; set; }

    public GenderTypes Gender { get; set; }

    public UpdateUserDto()
    {
        DisplayName = "";
        PhoneNumber = "";
        Avatar = "";
        Account = "";
        Address = new();
    }

    public UpdateUserDto(Guid id, string account, string? name, string displayName, string avatar, string? idCard, string? companyName, bool enabled, string phoneNumber, string? landline, string? email, AddressValueDto address, string? department, string? position, GenderTypes gender)
    {
        Id = id;
        Account = account;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Landline = landline;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        Gender = gender;
    }

    public static implicit operator UpdateUserDto(UserDetailDto user)
    {
        return new UpdateUserDto(user.Id, user.Account, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.CompanyName, user.Enabled, user.PhoneNumber ?? "", user.Landline, user.Email, user.Address, user.Department, user.Position, user.Gender);
    }
}
