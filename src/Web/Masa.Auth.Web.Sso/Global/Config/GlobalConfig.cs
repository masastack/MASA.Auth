// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global.Config;

public class GlobalConfig
{
    #region Field
    private CookieStorage? _cookieStorage;
    #endregion

    #region Property

    public I18n I18n { get; set; }

    public CultureInfo? Culture
    {
        get => I18n.Culture;
        set
        {
            if (I18n is not null)
            {
                I18n.SetCulture(value);
                OnLanguageChanged?.Invoke();
            }
        }
    }

    #endregion

    public GlobalConfig(CookieStorage cookieStorage, I18n i18n, IHttpContextAccessor httpContextAccessor)
    {
        _cookieStorage = cookieStorage;
        I18n = i18n;
        if (httpContextAccessor.HttpContext is not null)
        {
            Initialization(httpContextAccessor.HttpContext.Request.Cookies);
        }
    }

    #region event

    public delegate void GlobalConfigChanged();
    public event GlobalConfigChanged? OnLanguageChanged;

    #endregion

    #region Method

    public void Initialization(IRequestCookieCollection cookies)
    {
    }
    #endregion
}