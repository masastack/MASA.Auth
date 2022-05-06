// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global.Config
{
    public class GlobalConfig
    {
        #region Field

        private bool _isDark;
        private bool _expandOnHover;
        private CookieStorage? _cookieStorage;

        #endregion

        #region Property

        public static string IsDarkCookieKey { get; set; } = "GlobalConfig_IsDark";

        public static string ExpandOnHoverCookieKey { get; set; } = "GlobalConfig_ExpandOnHover";

        public I18nConfig? I18nConfig { get; set; }

        public string? Language
        {
            get => I18nConfig?.Language;
            set
            {
                if (I18nConfig is not null)
                {
                    I18nConfig.Language = value;
                    OnLanguageChanged?.Invoke();
                }
            }
        }

        public bool IsDark
        {
            get => _isDark;
            set
            {
                _isDark = value;
                _cookieStorage?.SetItemAsync(IsDarkCookieKey, value);
            }
        }

        public bool ExpandOnHover
        {
            get => _expandOnHover;
            set
            {
                _expandOnHover = value;
                _cookieStorage?.SetItemAsync(ExpandOnHoverCookieKey, value);
            }
        }

        #endregion

        public GlobalConfig(CookieStorage cookieStorage, I18nConfig i18nConfig, IHttpContextAccessor httpContextAccessor)
        {
            _cookieStorage = cookieStorage;
            I18nConfig = i18nConfig;
            if (httpContextAccessor.HttpContext is not null) Initialization(httpContextAccessor.HttpContext.Request.Cookies);
        }

        #region event

        public delegate void GlobalConfigChanged();
        public event GlobalConfigChanged? OnCurrentNavChanged;
        public event GlobalConfigChanged? OnLanguageChanged;

        #endregion

        #region Method

        public void Initialization(IRequestCookieCollection cookies)
        {
            _isDark = Convert.ToBoolean(cookies[IsDarkCookieKey]);
            _expandOnHover = Convert.ToBoolean(cookies[ExpandOnHoverCookieKey]);
        }
        #endregion
    }
}