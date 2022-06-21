// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IdCard { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    public string Landline { get; set; }

    public string Email { get; set; }

    public AddressValueDto Address { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string Password { get; set; }

    public GenderTypes Gender { get; set; }

    public UpdateUserDto()
    {
        Name = "";
        DisplayName = "";
        Avatar = "";
        IdCard = "";
        CompanyName = "";
        PhoneNumber = "";
        Email = "";
        Address = new();
        Department = "";
        Position = "";
        Password = "";
        Landline = "";
    }

    public UpdateUserDto(Guid id, string name, string displayName, string avatar, string idCard, string companyName, bool enabled, string phoneNumber, string email, AddressValueDto address, string department, string position, string password, GenderTypes genderType)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        Password = password;
        Gender = genderType;
        Landline = "";
    }

    public static implicit operator UpdateUserDto(UserDetailDto user)
    {
        return new UpdateUserDto(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.Address, user.Department, user.Position, user.Password, user.Gender);
    }
}
