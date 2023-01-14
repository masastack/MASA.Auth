// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class AddClientDto
{
    #region Basic
    public ClientTypes ClientType { get; set; } = ClientTypes.Web;

    public string ClientId { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> AllowedGrantTypes { get; set; } = new();

    public bool RequirePkce { get; set; } = true;
    #endregion

    #region Authentication
    /// <summary>
    /// call back url
    /// </summary>
    public List<string> RedirectUris { get; set; } = new();

    /// <summary>
    /// logout url
    /// </summary>
    public List<string> PostLogoutRedirectUris { get; set; } = new();
    #endregion

    #region Consent Screen
    public string ClientUri { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public bool RequireConsent { get; set; } = true;
    #endregion

    #region Client Credentials
    public bool RequireClientSecret { get; set; }
    #endregion

    #region ResourceScopes
    public List<string> AllowedScopes { get; set; } = new();
    #endregion

    public bool AllowOfflineAccess { get; set; } = true;
}
