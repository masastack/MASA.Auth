// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global.Config;

public class GlobalConfig
{
    #region Field

    private bool _isDark;
    private CookieStorage? _cookieStorage;

    #endregion

    #region Property

    public static string IsDarkCookieKey { get; set; } = "GlobalConfig_IsDark";

    public bool IsDark
    {
        get => _isDark;
        set
        {
            _isDark = value;
            _cookieStorage?.SetItemAsync(IsDarkCookieKey, value);
        }
    }

    #endregion

    public GlobalConfig(CookieStorage cookieStorage, IHttpContextAccessor httpContextAccessor)
    {
        _cookieStorage = cookieStorage;
        if (httpContextAccessor.HttpContext is not null) Initialization(httpContextAccessor.HttpContext.Request.Cookies);
    }

    #region event

    public delegate void GlobalConfigChanged();
    public event GlobalConfigChanged? OnLanguageChanged;

    #endregion

    #region Method

    public void Initialization(IRequestCookieCollection cookies)
    {
        _isDark = Convert.ToBoolean(cookies[IsDarkCookieKey]);
    }
    #endregion
}