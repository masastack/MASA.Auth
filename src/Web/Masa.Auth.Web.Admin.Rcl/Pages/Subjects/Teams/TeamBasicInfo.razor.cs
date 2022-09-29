// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamBasicInfo
{
    [Parameter]
    public TeamBasicInfoDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamBasicInfoDto> ValueChanged { get; set; }

    List<string> _colors = new List<string> { "purple", "green", "red", "blue", "orange" };
    List<TeamTypeDto> _teamTypes = new();

    private async Task OnNameChanged(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) &&
            (string.IsNullOrWhiteSpace(Value.Avatar.Name) || Value.Name.FirstOrDefault() != Value.Avatar.Name.FirstOrDefault()))
        {
            Value.Avatar.Name = name.FirstOrDefault().ToString();
        }
        Value.Name = name;
        await ValueChanged.InvokeAsync(Value);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _teamTypes = new()
            {
                new TeamTypeDto { Id = (int)TeamTypes.Ordinary, Name = T(TeamTypes.Ordinary.ToString()) }
            };
            if (string.IsNullOrEmpty(Value.Avatar.Color))
            {
                Value.Avatar.Color = _colors.First();
            }
        }
        base.OnAfterRender(firstRender);
    }
}
