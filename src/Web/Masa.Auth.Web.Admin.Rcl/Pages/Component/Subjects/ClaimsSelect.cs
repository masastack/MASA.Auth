// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class ClaimsSelect : SSelect<string, string, string>
{
    [Parameter]
    public List<string> Excludes { get; set; } = new();

    public Dictionary<string, string> Claims { get; set; } = UserClaims.Claims;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Items = Claims.Select(claim => claim.Key).ToList();
        ItemText = item => item;
        ItemValue = item => item;
        return base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        Items = Claims.Select(claim => claim.Key)
                      .Where(claim => Excludes.Any(key => key == claim) is false)
                      .ToList();
        base.OnParametersSet();
    }
}
