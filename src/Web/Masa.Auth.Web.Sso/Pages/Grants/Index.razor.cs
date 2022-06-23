// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Grants;

[SecurityHeaders]
[Authorize]
public partial class Index
{
    GrantViewModel _viewModel = new();

    public string ClientId { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var list = new List<GrantDetailViewModel>();
            foreach (var grant in SsoAuthenticationStateCache.Grants)
            {
                var client = await _clients.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resources = await _resources.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantDetailViewModel()
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            _viewModel = new GrantViewModel
            {
                Grants = list
            };
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnRevoke(string clientId)
    {
        await Interaction.RevokeUserConsentAsync(clientId);
        await Events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));
    }
}
