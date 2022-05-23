﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class SubjectDto
{
    public Guid SubjectId { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public BusinessTypes SubjectType { get; set; }

    public SubjectDto()
    {

    }

    public SubjectDto(Guid subjectId, string name, string displayName, string avatar, string phoneNumber, string email, BusinessTypes subjectType)
    {
        SubjectId = subjectId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
        SubjectType = subjectType;
    }
}


