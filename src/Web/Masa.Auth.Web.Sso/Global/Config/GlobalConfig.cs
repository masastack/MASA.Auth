// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global.Config;

public class GlobalConfig
{
    #region Property

    public I18n I18n { get; init; }

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

    public GlobalConfig(I18n i18n, IHttpContextAccessor httpContextAccessor)
    {
        I18n = i18n;
        I18n.SetCulture(CultureInfo.CurrentCulture);
    }

    #region event

    public delegate void GlobalConfigChanged();
    public event GlobalConfigChanged? OnLanguageChanged;

    #endregion
}