// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public abstract class AdminCompontentBase : NextTickComponentBase
{
    private GlobalConfig? _globalConfig;
    private AuthCaller? _authCaller;
    private NavigationManager? _navigationManager;

    [Inject]
    public AuthCaller AuthCaller
    {
        get => _authCaller ?? throw new Exception("please Inject AuthCaller!");
        set => _authCaller = value;
    }

    [Inject]
    public JsInitVariables JsInitVariables { get; set; } = default!;

    [CascadingParameter(Name = "Culture")]
    protected string Culture { get; set; } = null!;

    [Inject]
    public I18n I18n { get; set; } = default!;

    [Inject]
    public GlobalConfig GlobalConfig
    {
        get => _globalConfig ?? throw new Exception("please Inject GlobalConfig!");
        set => _globalConfig = value;
    }

    [Inject]
    public NavigationManager NavigationManager
    {
        get => _navigationManager ?? throw new Exception("please Inject NavigationManager!");
        set => _navigationManager = value;
    }

    [Inject]
    public DynamicTranslateProvider TranslateProvider { get; set; } = default!;

    [Inject]
    public IPopupService PopupService { get; set; } = default!;

    public bool Loading
    {
        get => GlobalConfig.Loading;
        set => GlobalConfig.Loading = value;
    }

    protected virtual string? PageName { get; set; }

    public string T(string key)
    {
        var val = "";
        if (string.IsNullOrEmpty(key))
        {
            return key;
        }
        if (PageName is not null)
        {
            val = I18n.T(PageName, key, false);
        }
        if (val.IsNullOrEmpty())
        {
            val = I18n.T(key);
        }
        return val;
    }

    public string T(string formatKey, params object?[] args)
    {
        return string.Format(T(formatKey), args);
    }

    protected string DT(string key)
    {
        return TranslateProvider.DT(key);
    }

    public async Task<bool> OpenConfirmDialog(string content)
    {
        return await PopupService.SimpleConfirmAsync(T("Operation confirmation"), content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content)
    {
        return await PopupService.SimpleConfirmAsync(title, content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content, AlertTypes type)
    {
        return await PopupService.SimpleConfirmAsync(title, content, type);
    }

    public void OpenInformationMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Info);
    }

    public void OpenSuccessMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Success);
    }

    public void OpenWarningMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Warning);
    }

    public void OpenErrorMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Error);
    }

    public List<KeyValuePair<string, TEnum>> GetEnumMap<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>().Select(e => new KeyValuePair<string, TEnum>(e.ToString(), e)).ToList();
    }

    public List<KeyValuePair<string, bool>> GetBooleanMap()
    {
        return new()
        {
            new(T("Enable"), true),
            new(T("Disabled"), false)
        };
    }

    public string ReturnNotNullValue(params string?[] values)
    {
        return values.FirstOrDefault(value => string.IsNullOrEmpty(value) is false) ?? "";
    }
}

