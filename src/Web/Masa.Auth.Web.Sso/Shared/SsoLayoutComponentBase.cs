// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Shared;

public class SsoLayoutComponentBase : LayoutComponentBase
{
    [Inject] public I18n LanguageProvider { get; set; } = null!;

    [Inject] public IPopupService PopupService { get; set; } = null!;

    public string T(string key)
    {
        return LanguageProvider.T(key) ?? key;
    }
}