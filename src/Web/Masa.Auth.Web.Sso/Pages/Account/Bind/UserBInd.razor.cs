// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Bind;

[AllowAnonymous]
public partial class UserBind
{
    private string? _captchaText;
    bool _smsloading;
    bool _loginLoading;

    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Inject]
    public IAuthClient AuthClient { get; set; } = default!;

    public RegisterThirdPartyUserModel UserModel { get; set; } = new();

    [NotNull]
    private string? CaptchaText
    {
        get => (_captchaText == "0" || _captchaText == null) ? T("Captcha") : _captchaText;
        set => _captchaText = value;
    }

    protected override async Task OnInitializedAsync()
    {
        var httpContext = HttpContextAccessor.HttpContext!;
        var identity = await httpContext.GetExternalIdentityAsync();
        UserModel = new RegisterThirdPartyUserModel
        {
            ThirdPartyIdpType = Enum.Parse<ThirdPartyIdpTypes>(identity.Issuer),
            ExtendedData = JsonSerializer.Serialize(identity),
            ThridPartyIdentity = identity.Subject,
            UserRegisterType = UserRegisterTypes.PhoneNumber,
            PhoneNumber = identity.PhoneNumber ?? "",
            Email = identity.Email ?? "",
            Account = identity.Account,
            DisplayName = identity.NickName ?? identity.Name,
            Avatar = identity.Picture
        };
        UserModel? user = default;
        if (string.IsNullOrEmpty(identity.PhoneNumber) is false)
        {
            user = await AuthClient.UserService.FindByPhoneNumberAsync(identity.PhoneNumber);
        }
        if (user is null && string.IsNullOrEmpty(identity.Email) is false)
        {
            user = await AuthClient.UserService.FindByEmailAsync(identity.Email);
        }
        if (user is not null)
        {
            UserModel.Avatar = user.Avatar;
            UserModel.DisplayName = user.DisplayName;
            UserModel.Account = user.Account;
            UserModel.PhoneNumber = user.PhoneNumber ?? "";
            UserModel.Email = user.Email ?? "";
        }
    }

    private async Task SendCaptcha(FormContext context)
    {
        if (CaptchaText != T("Captcha")) return;
        var field = context.EditContext.Field(nameof(UserModel.PhoneNumber));
        context.EditContext.NotifyFieldChanged(field);
        var result = context.EditContext.GetValidationMessages(field);
        if (result.Any() is false)
        {
            await AuthClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel()
            {
                PhoneNumber = UserModel.PhoneNumber,
                SendMsgCodeType = SendMsgCodeTypes.Bind
            });
            await PopupService.AlertAsync(T("The verification code is sent successfully"), AlertTypes.Success);
            int second = 60;
            while (second >= 0)
            {
                CaptchaText = second.ToString();
                StateHasChanged();
                second--;
                await Task.Delay(1000);
            }
        }
    }

    public async Task BindAsync(FormContext context)
    {
        if (context.Validate())
        {
            _loginLoading = true;
            await AuthClient.UserService.RegisterThirdPartyUserAsync(UserModel);
            Navigation.NavigateTo(AuthenticationExternalConstants.CallbackEndpoint, true);
            _loginLoading = false;
            await PopupService.ToastSuccessAsync("Login success");
        }
    }
}
