// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class LdapIdp : IdentityProvider
{
    public string ServerAddress { get; private set; } = null!;

    public int ServerPort { get; private set; }

    public bool IsSSL { get; private set; }

    public string BaseDn { get; private set; } = null!;

    public string UserSearchBaseDn { get; private set; } = string.Empty;

    public string GroupSearchBaseDn { get; private set; } = string.Empty;

    public string RootUserDn { get; private set; } = null!;

    public string RootUserPassword { get; private set; } = null!;

    private LdapIdp()
    {
        Name = "Ldap";
        DisplayName = "Ldap";
        Icon = "mdi-laptop";
        IdentificationType = IdentificationTypes.PhoneNumber;
    }

    public LdapIdp(string serverAddress, int serverPort, bool isSSL, string baseDn, string rootUserDn, string rootUserPassword) : this()
    {
        ServerAddress = serverAddress;
        ServerPort = serverPort;
        IsSSL = isSSL;
        BaseDn = baseDn;
        RootUserDn = rootUserDn;
        RootUserPassword = rootUserPassword;
    }

    public LdapIdp(string serverAddress, int serverPort, bool isSSL, string baseDn, string rootUserDn, string rootUserPassword, string userSearchBaseDn, string groupSearchBaseDn) : this(serverAddress, serverPort, isSSL, baseDn, rootUserDn, rootUserPassword)
    {
        GroupSearchBaseDn = groupSearchBaseDn;
        UserSearchBaseDn = userSearchBaseDn;
    }

    public void Update(LdapIdp ldapIdp)
    {
        ServerAddress = ldapIdp.ServerAddress;
        ServerPort = ldapIdp.ServerPort;
        IsSSL = ldapIdp.IsSSL;
        BaseDn = ldapIdp.BaseDn;
        RootUserDn = ldapIdp.RootUserDn;
        RootUserPassword = ldapIdp.RootUserPassword;
        GroupSearchBaseDn = ldapIdp.GroupSearchBaseDn;
        UserSearchBaseDn = ldapIdp.UserSearchBaseDn;
    }
}
