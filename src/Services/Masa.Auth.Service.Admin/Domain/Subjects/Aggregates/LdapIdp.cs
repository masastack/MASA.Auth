// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class LdapIdp : IdentityProvider
{
    public string ServerAddress { get; private set; } = null!;

    public int ServerPort { get; set; }

    public bool IsSSL { get; set; }

    public string BaseDn { get; set; } = null!;

    public string UserSearchBaseDn { get; private set; } = string.Empty;

    public string GroupSearchBaseDn { get; private set; } = string.Empty;

    public string RootUserDn { get; set; } = null!;

    public string RootUserPassword { get; set; } = null!;

    private LdapIdp()
    {
        Name = "Ldap";
        DisplayName = "Ldap";
        IdentificationType = IdentificationTypes.PhoneNumber;
    }
}
