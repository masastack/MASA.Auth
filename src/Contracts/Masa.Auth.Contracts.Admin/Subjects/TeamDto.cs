// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Description { get; set; }

    public int MemberCount { get; set; }

    public List<string> AdminAvatar { get; set; }

    public string Role { get; set; } = string.Empty;

    public string Modifier { get; set; }

    public DateTime? ModificationTime { get; set; }

    public TeamDto(Guid id, string name, string avatar, string description, int memberCount,
            List<string> adminAvatar, string modifier, DateTime? modificationTime)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Description = description;
        MemberCount = memberCount;
        AdminAvatar = adminAvatar;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }
}


