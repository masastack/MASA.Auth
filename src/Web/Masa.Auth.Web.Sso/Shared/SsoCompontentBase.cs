// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Security.Claims;

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
        public IHttpContextAccessor HttpContextAccessor { get; set; } = null!;

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        public IPopupService PopupService { get; set; } = null!;

        protected ClaimsPrincipal User
        {
            get
            {
                return HttpContextAccessor.HttpContext?.User ?? throw new UserFriendlyException("HttpContext user is null");
            }
        }

        public string T(string key)
        {
            return LanguageProvider.T(key) ?? key;
        }
    }
}