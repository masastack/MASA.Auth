// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Identity = Masa.Auth.Security.OAuth.Providers.Identity;

namespace Masa.Auth.Web.Sso.Pages.Account.Bind;

[AllowAnonymous]
public partial class UserBind
{
    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Inject]
    public IAuthClient AuthClient { get; set; } = default!;

    public Identity Identity { get; set; } = default!;

    public UserModel? UserModel { get; set; }

    public bool Visible { get; set; }

    public string Prompt { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        var user = User;
        var httpContext = HttpContextAccessor.HttpContext!;
        Identity = await httpContext.GetExternalIdentityAsync();
        if (string.IsNullOrEmpty(Identity.PhoneNumber) is false)
        {
            UserModel = await AuthClient.UserService.FindByPhoneNumberAsync(Identity.PhoneNumber);
            Prompt = $"It is found that there is a user whose mobile phone number is {Identity.PhoneNumber}, click to bind";
        }
        if (UserModel is null && string.IsNullOrEmpty(Identity.Email) is false)
        {
            UserModel = await AuthClient.UserService.FindByEmailAsync(Identity.Email);
            Prompt = $"It is found that there is a user whose mobile emai is {Identity.Email}, click to bind";
        }
        if (UserModel is null)
        {
            NavigateToRegister();
        }
    }

    public void NavigateToRegister()
    {
        Navigation.NavigateTo("");
    }

    public async Task BindAsync()
    {
        await AuthClient.UserService.AddThirdPartyUserAsync(new AddThirdPartyUserModel
        {
            ThridPartyIdentity = Identity.Subject,
            ExtendedData = JsonSerializer.Serialize(Identity),
            ThirdPartyIdpType = Enum.Parse<ThirdPartyIdpTypes>(Identity.Issuer),
            User = new AddUserModel
            {
                Email = Identity.Email,
                PhoneNumber = Identity.PhoneNumber,
            }
        });
        Navigation.NavigateTo(AuthenticationExternalConstants.CallbackEndpoint, true);
    }
}
