// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Shared;

public class SsoSectionComponentBase : ComponentBase
{
    I18n? _languageProvider;

    [Inject]
    public IPopupService PopupService { get; set; } = null!;

    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    [Inject]
    public SsoAuthenticationStateCache SsoAuthenticationStateCache { get; set; } = null!;

    [CascadingParameter]
    public I18n LanguageProvider
    {
        get
        {
            return _languageProvider ?? throw new Exception("please Inject I18n!");
        }
        set
        {
            _languageProvider = value;
        }
    }

    public string T(string key)
    {
        return LanguageProvider.T(key) ?? key;
    }
}
