// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AspNet.Security.OAuth.GitHub.GitHubAuthenticationConstants;

namespace Masa.Auth.Security.OAuth.Providers.GitHub;

public class GitHubBuilder : IIdentityBuilder, ILocalAuthenticationDefaultBuilder, IAuthenticationInject, IAuthenticationInstanceBuilder
{
    public string Scheme { get; } = GitHubAuthenticationDefaults.AuthenticationScheme;

    public AuthenticationDefaults AuthenticationDefaults { get; } = new AuthenticationDefaults
    {
        HandlerType = typeof(GitHubAuthenticationHandler),
        Scheme = GitHubAuthenticationDefaults.AuthenticationScheme,
        DisplayName = GitHubAuthenticationDefaults.DisplayName,
        Icon = "https://cdn.masastack.com/stack/auth/ico/github.svg",
        CallbackPath = GitHubAuthenticationDefaults.CallbackPath,
        Issuer = GitHubAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = GitHubAuthenticationDefaults.TokenEndpoint,
        UserInformationEndpoint = GitHubAuthenticationDefaults.UserInformationEndpoint,
        JsonKeyMap = new Dictionary<string, string>
        {
            [UserClaims.Subject] = "id",
            [UserClaims.Email] = "email",
            [UserClaims.WebSite] = "html_url",
            [UserClaims.Picture] = "avatar_url",
            [UserClaims.Account] = "login",
            [UserClaims.NickName] = "name",
        }
    };

    public Identity BuildIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);

        return identity;
    }

    public void Inject(AuthenticationBuilder builder, AuthenticationDefaults authenticationDefault)
    {
        builder.AddGitHub(authenticationDefault.Scheme, authenticationDefault.DisplayName, options =>
        {
            authenticationDefault.BindOAuthOptions(options);
            options.Scope.Add("user:email");
        });
    }

    public void InjectForHotUpdate(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GitHubAuthenticationOptions>, GitHubPostConfigureOptions>());
        serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GitHubAuthenticationOptions>, OAuthPostConfigureOptions<GitHubAuthenticationOptions, GitHubAuthenticationHandler>>());
    }

    public IAuthenticationHandler CreateInstance(IServiceProvider provider, AuthenticationDefaults authenticationDefaults)
    {
        var (options, loggerFactory, urlEncoder, systemClock) = CreateAuthenticationHandlerInstanceUtilities.BuilderParamter<GitHubAuthenticationOptions>(provider, authenticationDefaults.Scheme);
        authenticationDefaults.BindOAuthOptions(options.CurrentValue);
        if(options.CurrentValue.Scope.Any(scope => scope == "user:email") is false)
            options.CurrentValue.Scope.Add("user:email");

        return new GitHubAuthenticationHandler(options, loggerFactory, urlEncoder, systemClock);
    }
}
