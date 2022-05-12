// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class CustomLoginDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Title { get; set; } = "";

    public ClientDto Client { get; set; } = new();

    public bool Enabled { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; } = "";

    public string Modifier { get; set; } = "";

    public CustomLoginDto() { }

    public CustomLoginDto(int id, string name, string title, ClientDto client, bool enabled, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        Id = id;
        Name = name;
        Title = title;
        Client = client;
        Enabled = enabled;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}

