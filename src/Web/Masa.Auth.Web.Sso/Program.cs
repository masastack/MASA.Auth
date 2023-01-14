// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoInject();
builder.Services.AddMasaStackConfig();
var masaStackConfig = builder.Services.GetMasaStackConfig();

X509Certificate2 serverCertificate;
if (string.IsNullOrEmpty(masaStackConfig.TlsName))
{
    serverCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN");
}
else
{
    serverCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
}

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    {
        options.ServerCertificate = serverCertificate;
        options.CheckCertificateRevocation = false;
    });
});

// Add services to the container.
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc(masaStackConfig.GetDccMiniOptions<DccOptions>());
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor(builder =>
{
    builder.ConfigureTheme(theme =>
    {
        theme.Dark = true;
        theme.Themes.Light.Primary = "#4318FF";
        theme.Themes.Light.Accent = "#4318FF";
    });
}).AddI18nForServer("wwwroot/i18n");
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();
builder.Services.AddMasaIdentity();
builder.Services.AddScoped<IEnvironmentProvider, SsoEnvironmentProvider>();

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
            Port=   masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword
};

builder.Services.AddAuthClient(authDomain, redisOption);

builder.Services.AddMcClient(masaStackConfig.GetMcServiceDomain());
builder.Services.AddPmClient(masaStackConfig.GetPmServiceDomain());

builder.Services.AddTransient<IConsentMessageStore, ConsentResponseStore>();
builder.Services.AddSameSiteCookiePolicy();
builder.Services.AddMultilevelCache(distributedCacheOptions =>
{
    distributedCacheOptions.UseStackExchangeRedisCache(redisOption);
});
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
    .AddExtensionGrantValidator<LoclaPhoneNumberGrantValidator>();

//#if DEBUG
//identityServerBuilder.AddDeveloperSigningCredential();
//#else
//identityServerBuilder.AddSigningCredential(serverCertificate);
//#endif

builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect((ConfigurationOptions)redisOption));

builder.Services.AddHotUpdateAuthenticationExternal<AuthenticationExternalHandler, RemoteAuthenticationDefaultsProvider>();

builder.Services.AddScoped<IUserSession, ClientUserSession>();

builder.Services.AddSingleton<SsoAuthenticationStateCache>();
builder.Services.AddScoped<AuthenticationStateProvider, SsoAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

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