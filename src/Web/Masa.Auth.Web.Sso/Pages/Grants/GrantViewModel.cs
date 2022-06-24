// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Grants;

public class GrantViewModel
{
    public IEnumerable<GrantDetailViewModel> Grants { get; set; } = new List<GrantDetailViewModel>();
}

public class GrantDetailViewModel
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientUrl { get; set; } = string.Empty;
    public string ClientLogoUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? Expires { get; set; }
    public IEnumerable<string> IdentityGrantNames { get; set; } = new List<string>();
    public IEnumerable<string> ApiGrantNames { get; set; } = new List<string>();
}