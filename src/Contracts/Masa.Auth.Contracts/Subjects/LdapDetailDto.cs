// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class LdapDetailDto
{
    public string ServerAddress { get; set; } = string.Empty;

    public int ServerPort { get; set; } = 389;

    public bool IsLdaps { get; set; }

    public string BaseDn { get; set; } = string.Empty;

    public string UserSearchBaseDn { get; set; } = string.Empty;

    public string GroupSearchBaseDn { get; set; } = string.Empty;

    public string RootUserDn { get; set; } = null!;

    public string RootUserPassword { get; set; } = null!;
}
