namespace Masa.Stack.Components.Extensions.OpenIdConnect;

public static class ServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddMasaOpenIdConnectAsync(
        this WebAssemblyHostBuilder builder,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("$public.OIDC").Get<MasaOpenIdConnectOptions>();
        return await builder.AddMasaOpenIdConnectAsync(options);
    }

    public static async Task<IServiceCollection> AddMasaOpenIdConnectAsync(
        this WebAssemblyHostBuilder builder,
        MasaOpenIdConnectOptions masaOpenIdConnectOptions)
    {
        builder.Services.AddSingleton(masaOpenIdConnectOptions);
        builder.Services.AddSingleton<LogoutSessionManager>();
        builder.Services.TryAddScoped<CookieStorage>();
        return await builder.AddMasaOpenIdConnectAsync(
            masaOpenIdConnectOptions.Authority,
            masaOpenIdConnectOptions.ClientId,
            masaOpenIdConnectOptions.ClientSecret,
            masaOpenIdConnectOptions.Scopes.ToArray());
    }

    private static async Task<IServiceCollection> AddMasaOpenIdConnectAsync(
        this WebAssemblyHostBuilder builder,
        string authority,
        string clientId,
        string clientSecret,
        params string[] scopes)
    {
        var cookieStorage = builder.Services.BuildServiceProvider().GetRequiredService<CookieStorage>();
        var environment = await cookieStorage.GetAsync(Consts.ENVIRONMENT);

        builder.Services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.Authority = authority;
            options.ProviderOptions.ClientId = clientId;
            options.ProviderOptions.ResponseType = "code";
            options.ProviderOptions.DefaultScopes.Clear();
            foreach (var scope in scopes)
            {
                options.ProviderOptions.DefaultScopes.Add(scope);
            }
            if (!string.IsNullOrEmpty(environment))
            {
                options.ProviderOptions.AdditionalProviderParameters[Consts.ENVIRONMENT] = environment;
            }
        });

        builder.Services.AddAuthorizationCore(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        return builder.Services;
    }
}
