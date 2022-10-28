// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class UpdateThirdPartyIdpDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyIdpId { get; set; }

    private ThirdPartyIdpDetailDto ThirdPartyIdpDetail { get; set; } = new();

    private UpdateThirdPartyIdpDto ThirdPartyIdp { get; set; } = new();

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override string? PageName { get; set; } = "ThirdPartyIdpBlock";

    List<KeyValue> AdvancedConfig { get; set; } = new();

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetThirdPartyIdpDetailAsync();
        }
    }

    public async Task GetThirdPartyIdpDetailAsync()
    {
        ThirdPartyIdpDetail = await ThirdPartyIdpService.GetDetailAsync(ThirdPartyIdpId);
        ThirdPartyIdp = ThirdPartyIdpDetail;
        AdvancedConfig = ThirdPartyIdpDetail.JsonKeyMap.Select(kv => new KeyValue
        {
            Key = kv.Key,
            Value = kv.Value
        }).ToList();
    }

    public async Task UpdateThirdPartyIdpAsync(FormContext context)
    {
        if (string.IsNullOrEmpty(ThirdPartyIdp.Icon))
        {
            OpenErrorMessage(T("Please upload ThirdPartyIdp icon"));
            return;
        }
        if (AdvancedConfig.Any(config => config.IsDefault()))
        {
            OpenErrorMessage(T("Please complete advanced configuration"));
            return;
        }
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            ThirdPartyIdp.JsonKeyMap = AdvancedConfig.ToDictionary(config => config.Key, config => config.Value);
            await ThirdPartyIdpService.UpdateAsync(ThirdPartyIdp);
            OpenSuccessMessage(T("Edit thirdPartyIdp data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}

