// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public string VerifyFile { get; set; } = "";

    public bool Enabled { get; set; }

    public AuthenticationTypes VerifyType { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public ThirdPartyIdpDto()
    {

    }

    public ThirdPartyIdpDto(Guid id, string name, string displayName, string clientId, string clientSecret, string url, string icon, string verifyFile, bool enabled, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyFile = verifyFile;
        Enabled = enabled;
        VerifyType = authenticationType;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
    }
}

