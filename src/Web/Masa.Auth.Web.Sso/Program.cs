// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoInject();
builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

// Add services to the container.
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
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

var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();

#if DEBUG
builder.Services.AddAuthClient(publicConfiguration, "http://localhost:18002/");
#else
builder.Services.AddAuthClient(publicConfiguration);
#endif

builder.Services.AddMcClient(publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url"));
builder.Services.AddPmClient(publicConfiguration.GetValue<string>("$public.AppSettings:PmClient:Url"));

builder.Services.AddTransient<IConsentMessageStore, ConsentResponseStore>();
builder.Services.AddSameSiteCookiePolicy();
var redisOption = publicConfiguration.GetSection("$public.RedisConfig").Get<RedisConfigurationOptions>();
builder.Services.AddMultilevelCache(distributedCacheOptions =>
{
    distributedCacheOptions.UseStackExchangeRedisCache(redisOption);
});
builder.Services.AddOidcCacheStorage(redisOption)
    .AddIdentityServer(options =>
    {
        options.UserInteraction.ErrorUrl = "/error/500";
        options.Events.RaiseSuccessEvents = true;
    })
    .AddDeveloperSigningCredential()
    .AddClientStore<MasaClientStore>()
    .AddResourceStore<MasaResourceStore>()
    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddProfileService<UserProfileService>()
    .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
    .AddExtensionGrantValidator<PhoneCodeGrantValidator>()
    .AddExtensionGrantValidator<LoclaPhoneNumberGrantValidator>();

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