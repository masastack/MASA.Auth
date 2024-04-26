// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoInject();

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.Auth, MasaStackApp.SSO);
var masaStackConfig = builder.Services.GetMasaStackConfig();

// Add services to the container.
builder.Services.AddScoped<EnvironmentDataService>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor(builder =>
{
    builder.ConfigureTheme(theme =>
    {
        theme.Dark = false;
        theme.Themes.Light.Primary = "#4318FF";
        theme.Themes.Light.Accent = "#4318FF";
    });
}).AddI18nForServer("wwwroot/i18n");
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();
builder.Services.AddMasaIdentity();

var authDomain = masaStackConfig.GetAuthServiceDomain();

#if DEBUG
authDomain = "http://localhost:18002";
#endif

var redisOption = new RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions> {
        new RedisServerOptions()
        {
            Host= masaStackConfig.RedisModel.RedisHost,
            Port= masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword
};

builder.Services.AddAuthClient(authDomain, redisOption);

builder.Services.AddMcClient(masaStackConfig.GetMcServiceDomain());
builder.Services.AddPmClient(masaStackConfig.GetPmServiceDomain());

builder.Services.AddTransient<IConsentMessageStore, ConsentResponseStore>();
builder.Services.AddScoped<IEventSink, IdentityServerEventSink>();
builder.Services.AddSameSiteCookiePolicy();
builder.Services.AddMultilevelCache(distributedCacheOptions =>
{
    distributedCacheOptions.UseStackExchangeRedisCache(redisOption);
});
builder.Services.AddScoped<CookieStorage>();
var identityServerBuilder = builder.Services.AddOidcCacheStorage(redisOption)
    .AddIdentityServer(options =>
    {
        options.UserInteraction.ErrorUrl = "/error/500";
        options.Events.RaiseSuccessEvents = true;
    })
    .AddClientStore<MasaClientStore>()
    .AddResourceStore<MasaResourceStore>()
    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddProfileService<UserProfileService>()
    .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
    .AddExtensionGrantValidator<PhoneCodeGrantValidator>()
    .AddExtensionGrantValidator<LocalPhoneNumberGrantValidator>()
    .AddExtensionGrantValidator<ThirdPartyIdpGrantValidator>()
    .AddExtensionGrantValidator<LdapGrantValidator>()
    .AddExtensionGrantValidator<ImpersonationGrantValidator>();

if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();
}
else
{
    var serverCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
    builder.WebHost.UseKestrel(option =>
    {
        option.ConfigureHttpsDefaults(options =>
        {
            options.ServerCertificate = serverCertificate;
            options.CheckCertificateRevocation = false;
        });
    });
    identityServerBuilder.AddSigningCredential(serverCertificate);
    //tsc
    builder.Services.AddObservable(builder.Logging, () =>
    {
        return new MasaObservableOptions
        {
            ServiceNameSpace = builder.Environment.EnvironmentName,
            ServiceVersion = masaStackConfig.Version,
            ServiceName = masaStackConfig.GetId(MasaStackProject.Auth, MasaStackApp.SSO),
            Layer = masaStackConfig.Namespace,
            ServiceInstanceId = builder.Configuration.GetValue<string>("HOSTNAME")
        };
    }, () =>
    {
        return masaStackConfig.OtlpUrl;
    }, true);
}

builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect((ConfigurationOptions)redisOption));

builder.Services.AddHotUpdateAuthenticationExternal<AuthenticationExternalHandler, RemoteAuthenticationDefaultsProvider>();

builder.Services.AddScoped<IUserSession, ClientUserSession>();

builder.Services.AddSingleton<SsoAuthenticationStateCache>();
builder.Services.AddScoped<AuthenticationStateProvider, SsoAuthenticationStateProvider>();
builder.Services.AddLadpContext();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

//todo https://github.com/jsakamoto/Toolbelt.Blazor.HeadElement
app.Use(async (context, next) =>
{
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
    if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    }

    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
    if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
    {
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    }
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
    var csp = "default-src * 'self'; object-src 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";//frame-ancestors 'none'; 
    // also consider adding upgrade-insecure-requests once you have HTTPS in place for production
    csp += "upgrade-insecure-requests;";
    csp += "img-src * 'self' data:; script-src 'self' 'unsafe-inline' 'unsafe-eval' * data:;style-src  'self' 'unsafe-inline' *;frame-src *";

    // once for standards compliant browsers
    if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
    {
        context.Response.Headers.Add("Content-Security-Policy", csp);
    }
    // and once again for IE
    if (!context.Response.Headers.ContainsKey("X-Content-Security-Policy"))
    {
        context.Response.Headers.Add("X-Content-Security-Policy", csp);
    }

    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
    var referrer_policy = "no-referrer";
    if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
    {
        context.Response.Headers.Add("Referrer-Policy", referrer_policy);
    }
    await next();
});

app.UseCookiePolicy();
app.UseIdentityServer();
// This cookie policy fixes login issues with Chrome 80+ using HHTP
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication()
   .UseAuthorizationExternal();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
