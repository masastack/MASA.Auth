// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class AddThirdPartyIdpDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyIdpId { get; set; }

    private AddThirdPartyIdpDto ThirdPartyIdp { get; set; } = new();

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override string? PageName { get; set; } = "ThirdPartyIdpBlock";

    List<KeyValue> AdvancedConfig { get; set; } = new();

    private async Task UpdateVisible(bool visible)
    {
        if (!Visible)
        {
            ThirdPartyIdp = new();
            AdvancedConfig = new();
        }
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    public async Task AddThirdPartyIdpAsync(FormContext context)
    {
        if (string.IsNullOrEmpty(ThirdPartyIdp.Icon))
        {
            OpenErrorMessage(T("Please upload ThirdPartyIdp icon"));
            return;
        }
        if(AdvancedConfig.Any(config => config.IsDefault()))
        {
            OpenErrorMessage(T("Please complete advanced configuration"));
            return;
        }
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            ThirdPartyIdp.JsonKeyMap = AdvancedConfig.ToDictionary(config => config.Key,config => config.Value);
            await ThirdPartyIdpService.AddAsync(ThirdPartyIdp);
            OpenSuccessMessage(T("Add thirdPartyIdp success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }

    void ThirdPartyIdpValueChanged(ThirdPartyIdpModel value)
    {
        ThirdPartyIdp.ThirdPartyIdpType = Enum.Parse<ThirdPartyIdpTypes>(value.Name);
        ThirdPartyIdp.Icon = value.Icon;
        ThirdPartyIdp.Name = value.Name;
        ThirdPartyIdp.DisplayName = value.DisplayName;
        ThirdPartyIdp.CallbackPath = value.CallbackPath;
        ThirdPartyIdp.AuthorizationEndpoint = value.AuthorizationEndpoint;
        ThirdPartyIdp.TokenEndpoint = value.TokenEndpoint;
        ThirdPartyIdp.UserInformationEndpoint = value.UserInformationEndpoint;
        ThirdPartyIdp.MapAll = value.MapAll;
        AdvancedConfig = value.JsonKeyMap.Select(kv => new KeyValue 
        {
            Key = kv.Key,
            Value = kv.Value
        }).ToList();
    }
}

