// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityServer4.EntityFramework.Services;
using Masa.Auth.Web.Sso.Infrastructure.Services;
using Masa.Auth.Web.Sso.Infrastructure.Validation;
using Masa.Contrib.BasicAbility.Auth;
using Masa.Contrib.Oidc.EntityFramework;
using Masa.Utils.Caching.Redis.Models;
using Client = Masa.BuildingBlocks.Oidc.Domain.Entities.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor(builder =>
{
    builder.UseTheme(option =>
    {
        option.Primary = "#4318FF";
        option.Accent = "#4318FF";
    }
    );
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();

builder.Services.AddAuthClient("https://localhost:18102");

var client = new Client(Masa.BuildingBlocks.Oidc.Domain.Enums.ClientTypes.Web, "masa.auth.admin.web", "Masa Auth Web");
client.SetAllowedScopes(new List<string> { "openid", "profile", "email" });
client.SetPostLogoutRedirectUris(new List<string> { "https://localhost:18100/signout-callback-oidc" });
client.SetRedirectUris(new List<string> { "https://localhost:18100/signin-oidc" });

//builder.WebHost.UseKestrel(option =>
//{
//    option.ConfigureHttpsDefaults(options =>
//    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
//});

builder.Services.AddOidcCacheStorage(builder.Configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>())
.AddOidcDbContext(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
}).SeedClientData(new List<Client> { client })
.AddIdentityServer(options =>
{
    options.UserInteraction.ErrorUrl = "/error/500";
})
.AddDeveloperSigningCredential()
.AddClientStore<MasaClientStore>()
.AddResourceStore<MasaResourceStore>()
.AddCorsPolicyService<CorsPolicyService>()
.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
.AddProfileService<UserProfileService>();

builder.Services.AddPmClient(builder.Configuration.GetValue<string>("PmClient:Url"));

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
}

app.UseIdentityServer();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();