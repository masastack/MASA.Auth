// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ImpersonateOutput
{
    public string ImpersonationToken { get; set; } = string.Empty;
}