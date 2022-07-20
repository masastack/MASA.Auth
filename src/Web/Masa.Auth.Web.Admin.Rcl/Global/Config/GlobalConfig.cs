// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Masa.Auth.Web.Admin.Rcl.Global.Config
{
    public class GlobalConfig
    {
        #region Field

        private bool _Loading;
        private List<string>? _elementPermissions;
        private CookieStorage _cookieStorage;

        #endregion

        #region Property

        public static string ElementPermissionsCookieKey { get; set; } = "element_permissions";

        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading != value)
                {
                    _Loading = value;
                    OnLoadingChanged?.Invoke(_Loading);
                }
            }
        }

        public List<string>? ElementPermissions
        {
            get => _elementPermissions;
            set
            {
                _elementPermissions = value;
                _cookieStorage.SetItemAsync(ElementPermissionsCookieKey, JsonSerializer.Serialize(value));
            }
        }

        #endregion


        #region event

        public delegate void GlobalConfigChanged();
        public delegate void LoadingChanged(bool Loading);

        public event LoadingChanged? OnLoadingChanged;

        #endregion

        public GlobalConfig(CookieStorage cookieStorage, IHttpContextAccessor httpContextAccessor)
        {
            _cookieStorage = cookieStorage;
            if (httpContextAccessor.HttpContext != null)
            {
                Initialization(httpContextAccessor.HttpContext.Request.Cookies);
            }
        }

        void Initialization(IRequestCookieCollection cookies)
        {
            if (cookies.TryGetValue(ElementPermissionsCookieKey, out string? value) && value != null)
            {
                _elementPermissions = JsonSerializer.Deserialize<List<string>>(value);
            }
        }
    }
}