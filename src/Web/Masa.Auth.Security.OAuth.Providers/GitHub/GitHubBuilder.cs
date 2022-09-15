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
        Icon = "https://masa-cdn-dev.oss-cn-hangzhou.aliyuncs.com/app.ico",
        CallbackPath = GitHubAuthenticationDefaults.CallbackPath,
        Issuer = GitHubAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = GitHubAuthenticationDefaults.TokenEndpoint,
        UserInformationEndpoint = GitHubAuthenticationDefaults.UserInformationEndpoint,
        JsonKeyMap = new Dictionary<string, string>
        {
            [UserClaims.WebSite] = "html_url",
            [UserClaims.Picture] = "avatar_url",
            [UserClaims.Account] = "login"
        }
    };

    public Identity BuildIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);
        identity.NickName = principal.FindFirstValue(Claims.Name);
        identity.WebSite = principal.FindFirstValue(Claims.Url);

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
        var (options, loggerFactory, urlEncoder, systemClock) = CreateAuthenticationHandlerInstanceUtilities.BuilderParamter<GitHubAuthenticationOptions>(provider);
        authenticationDefaults.BindOAuthOptions(options.CurrentValue);
        options.CurrentValue.Scope.Add("user:email");
        return new GitHubAuthenticationHandler(options, loggerFactory, urlEncoder, systemClock);
    }
}
