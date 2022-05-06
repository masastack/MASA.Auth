// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Authentication.Components
{
    public partial class Register
    {
        private bool _show;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public bool HideLogo { get; set; }

        [Parameter]
        public double Width { get; set; } = 410;

        [Parameter]
        public StringNumber? Elevation { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> SlinUpClick { get; set; }

        [Parameter]
        public string SignInRoute { get; set; } = $"pages/authentication/login-v1";
    }
}