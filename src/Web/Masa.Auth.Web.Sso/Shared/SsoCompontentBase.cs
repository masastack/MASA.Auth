// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Shared
{
    public abstract class SsoCompontentBase : ComponentBase
    {
        private I18n? _languageProvider;

        [Inject]
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

        [Inject]
        public IIdentityServerInteractionService Interaction { get; set; } = null!;

        [Inject]
        public IEventService Events { get; set; } = null!;

        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public IPopupService PopupService { get; set; } = null!;

        [Inject]
        public SsoAuthenticationStateCache SsoAuthenticationStateCache { get; set; } = null!;

        protected ClaimsPrincipal User
        {
            get
            {
                return _authenticationStateProvider.GetAuthenticationStateAsync().Result.User;
            }
        }

        public string T(string key)
        {
            return LanguageProvider.T(key) ?? key;
        }
    }
}