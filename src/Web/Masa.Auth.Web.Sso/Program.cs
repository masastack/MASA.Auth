// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

// Add services to the container.
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

builder.Services.AddMasaIdentityModel();
builder.Services.AddScoped<IEnvironmentProvider, SsoEnvironmentProvider>();
builder.Services.AddAuthClient(builder.Configuration.GetValue<string>("AuthServiceUrl"));
builder.Services.AddMcClient(builder.Configuration.GetValue<string>("McServiceUrl"));
builder.Services.AddPmClient(builder.Configuration.GetValue<string>("PmServiceUrl"));

builder.Services.AddSameSiteCookiePolicy();
var redisOption = builder.Configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();