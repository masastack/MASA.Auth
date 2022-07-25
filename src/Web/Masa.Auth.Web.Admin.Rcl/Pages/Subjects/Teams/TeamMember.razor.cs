// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamMember
{
    [EditorRequired]
    [Parameter]
    public TeamPersonnelDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamPersonnelDto> ValueChanged { get; set; }

    [Parameter]
    public bool Preview { get; set; }

    [Parameter]
    public EventCallback<bool> PreviewChanged { get; set; }

    private RoleSelectForTeam? RoleSelectRef { get; set; }
}
