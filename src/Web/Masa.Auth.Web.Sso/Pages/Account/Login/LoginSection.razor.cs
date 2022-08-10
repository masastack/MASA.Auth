// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Mc.Enum;
using Masa.BuildingBlocks.StackSdks.Mc.Model;

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class LoginSection
{

    [CascadingParameter(Name = "HttpContext")]
    public HttpContext? HttpContext { get; set; }

    [CascadingParameter]
    public string ReturnUrl { get; set; } = string.Empty;

    [Parameter]
    public string LoginHint { get; set; } = string.Empty;

    LoginInputModel _inputModel = new();
    MForm _loginForm = null!;
    bool _showPwd, _canSmsCode = true;
    List<EnvironmentModel> _environments = new();
    System.Timers.Timer? _timer;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval;

    protected override void OnInitialized()
    {
        if (_timer == null)
        {
            _timer = new(1000 * 1);
            _timer.Elapsed += Timer_Elapsed;
        }
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _environments = await _pmClient.EnvironmentService.GetListAsync();
            _inputModel = new LoginInputModel
            {
                ReturnUrl = ReturnUrl,
                UserName = LoginHint,
                Environment = _environments.FirstOrDefault()?.Name ?? ""
            };
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _ = InvokeAsync(() =>
        {
            _smsCodeTime--;
            if (_smsCodeTime == 0)
            {
                _timer?.Stop();
                _canSmsCode = true;
                _smsCodeTime = LoginOptions.GetSmsCodeInterval;
            }
            StateHasChanged();
        });
    }

    private async Task LoginHandler()
    {
        var d = await _loginForm.ValidateAsync();
        //validate
        if (await _loginForm.ValidateAsync())
        {
            if (HttpContext != null)
            {
                await HttpContext.SignOutAsync();
            }

            var msg = await _js.InvokeAsync<string>("login", _inputModel);
            if (!string.IsNullOrEmpty(msg))
            {
                await PopupService.AlertAsync(msg, AlertTypes.Error);
            }
            else
            {
                if (SsoUrlHelper.IsLocalUrl(_inputModel.ReturnUrl))
                {
                    Navigation.NavigateTo(_inputModel.ReturnUrl, true);
                }
                else if (string.IsNullOrEmpty(_inputModel.ReturnUrl))
                {
                    Navigation.NavigateTo("/", true);
                }
                else
                {
                    await PopupService.AlertAsync("invalid return URL", AlertTypes.Error);
                }
            }
        }
    }

    private async Task Cancel()
    {
        // check if we are in the context of an authorization request
        var context = SsoAuthenticationStateCache.GetAuthorizationContext(_inputModel.ReturnUrl);

        if (context != null)
        {
            // if the user cancels, send a result back into IdentityServer as if they 
            // denied the consent (even if this client does not require consent).
            // this will send back an access denied OIDC error response to the client.
            await Interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                Navigation.LoadingPage(_inputModel.ReturnUrl);
                return;
            }
            Navigation.NavigateTo(_inputModel.ReturnUrl, true);
            return;
        }
        else
        {
            // since we don't have a valid context, then we just go back to the home page
            Navigation.NavigateTo("/", true);
            return;
        }
    }

    private async Task GetSmsCode()
    {
        var d = await _loginForm.ValidateAsync();
        if (string.IsNullOrWhiteSpace(_inputModel.PhoneNumber) || !Regex.IsMatch(_inputModel.PhoneNumber,
                LoginOptions.PhoneRegular))
        {
            await PopupService.AlertAsync(T("PhoneNumberPrompt"), AlertTypes.Error);
            return;
        }
        var code = new Random().Next(100000, 999999);
        await _mcClient.MessageTaskService.SendTemplateMessageAsync(new SendTemplateMessageModel
        {
            ChannelCode = _configuration.GetValue<string>("Sms:ChannelCode"), //todo dcc
            ChannelType = ChannelTypes.Sms,
            TemplateCode = _configuration.GetValue<string>("Sms:TemplateCode"),
            ReceiverType = SendTargets.Assign,
            Receivers = new List<MessageTaskReceiverModel>
            {
                new MessageTaskReceiverModel
                {
                    Type = MessageTaskReceiverTypes.User,
                    PhoneNumber = _inputModel.PhoneNumber
                }
            },
            Variables = new ExtraPropertyDictionary(new Dictionary<string, object>
            {
                ["code"] = code,
            })
        });
        await _distributedCacheClient.SetAsync(CacheKey.GetSmsCodeKey(_inputModel.PhoneNumber), code
            , new Utils.Caching.Core.Models.CombinedCacheEntryOptions<int>
            {
                DistributedCacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = LoginOptions.SmsCodeExpire
                }
            });
        _canSmsCode = false;
        _timer?.Start();
    }

    private async Task KeyDownHandler(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await LoginHandler();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
