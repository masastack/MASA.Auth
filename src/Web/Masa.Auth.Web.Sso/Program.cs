// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

// Add services to the container.
builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
var publicConfiguration = builder.GetMasaConfiguration().ConfigurationApi.GetPublic();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor(builder =>
{
    builder.Theme.Primary = "#4318FF";
    builder.Theme.Accent = "#4318FF";
}).AddI18nForServer("wwwroot/i18n");
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddMasaIdentityModel();
builder.Services.AddScoped<IEnvironmentProvider, SsoEnvironmentProvider>();
builder.Services.AddAuthClient(publicConfiguration.GetValue<string>("$public.AppSettings:AuthClient:LocalUrl"));
builder.Services.AddMcClient(publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url"));
builder.Services.AddPmClient(publicConfiguration.GetValue<string>("$public.AppSettings:PmClient:Url"));

builder.Services.AddTransient<IConsentMessageStore, ConsentResponseStore>();
builder.Services.AddSameSiteCookiePolicy();
var redisOption = builder.GetMasaConfiguration().Local.GetSection("RedisConfig").Get<RedisConfigurationOptions>();
builder.Services.AddMasaRedisCache(redisOption);
builder.Services.AddOidcCacheStorage(redisOption)
    .AddIdentityServer(options =>
    {
        options.UserInteraction.ErrorUrl = "/error/500";
    })
    .AddDeveloperSigningCredential()
    .AddClientStore<MasaClientStore>()
    .AddResourceStore<MasaResourceStore>()
    .AddCorsPolicyService<CorsPolicyService>()
    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddProfileService<UserProfileService>()
    .AddCustomTokenRequestValidator<CustomTokenRequestValidator>();

builder.Services.AddAuthenticationExternal<AuthenticationExternalHandler>()
                .AddDefaultGitHub(options =>
                {
                    options.ClientId = "49e302895d8b09ea5656";
                    options.ClientSecret = "98f1bf028608901e9df91d64ee61536fe562064b";
                })
                .AddDefaultWeChat(options =>
                {
                });

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