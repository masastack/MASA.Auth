// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Device;

public class ViewModel
{
    public string ClientName { get; set; } = string.Empty;
    public string ClientUrl { get; set; } = string.Empty;
    public string ClientLogoUrl { get; set; } = string.Empty;
    public bool AllowRememberConsent { get; set; }

    public IEnumerable<ScopeViewModel> IdentityScopes { get; set; } = new List<ScopeViewModel>();
    public IEnumerable<ScopeViewModel> ApiScopes { get; set; } = new List<ScopeViewModel>();
}

public class ScopeViewModel
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Emphasize { get; set; }
    public bool Required { get; set; }
    public bool Checked { get; set; }
}