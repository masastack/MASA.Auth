// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserSelectDto : AutoCompleteDocument<Guid>
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Account { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Avatar { get; set; }

    public UserSelectDto()
    {
        Name = "";
        DisplayName = "";
        Account = "";
        PhoneNumber = "";
        Email = "";
        Avatar = "";
    }

    public UserSelectDto(Guid userId, string name, string displayName, string account, string phoneNumber, string email, string avatar) : base(userId.ToString(), $"{name},{account},{phoneNumber},{email}", userId)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Account = account;
        PhoneNumber = phoneNumber;
        Email = email;
        Avatar = avatar;
    }
}
