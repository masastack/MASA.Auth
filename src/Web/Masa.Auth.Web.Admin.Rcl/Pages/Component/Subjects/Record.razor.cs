// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class Record
{
    [Parameter]
    public string Creator { get; set; } = "";

    [Parameter]
    public DateTime CreationTime { get; set; }

    [Parameter]
    public string Modifier { get; set; } = "";

    [Parameter]
    public DateTime? ModificationTime { get; set; }
}
