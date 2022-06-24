// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Consent;

public class InputModel
{
    public string Button { get; set; } = string.Empty;
    public IEnumerable<string> ScopesConsented { get; set; } = new List<string>();
    public bool RememberConsent { get; set; } = true;
    public string ReturnUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}