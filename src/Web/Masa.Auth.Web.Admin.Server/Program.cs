// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

builder.Services.AddObservable(builder.Logging, builder.Configuration, true);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

#if DEBUG
builder.AddMasaStackComponentsForServer("wwwroot/i18n", "http://localhost:18002/");
#else
builder.AddMasaStackComponentsForServer();
#endif
var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();
#if DEBUG
builder.Services.AddAuthApiGateways(option => option.AuthServiceBaseAddress = "http://localhost:18002/");
#else
builder.Services.AddAuthApiGateways(option => option.AuthServiceBaseAddress = publicConfiguration.GetValue<string>("$public.AppSettings:AuthClient:Url"));
#endif

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();

builder.Services.AddScoped<IPermissionValidator, PermissionValidator>();
builder.Services.AddSingleton<AddStaffValidator>();
builder.Services.AddTypeAdapter();

MasaOpenIdConnectOptions masaOpenIdConnectOptions;

IdentityModelEventSource.ShowPII = true;

#if DEBUG
masaOpenIdConnectOptions = new MasaOpenIdConnectOptions
{
    Authority = "http://localhost:18200",
    ClientId = "masa.stack.web-development",
    Scopes = new List<string> { "offline_access" }
};
builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions);
#else
masaOpenIdConnectOptions = publicConfiguration.GetSection("$public.OIDC").Get<MasaOpenIdConnectOptions>();
builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions);
#endif
builder.Services.AddJwtTokenValidator(options =>
{
    options.AuthorityEndpoint = masaOpenIdConnectOptions.Authority;
}, refreshTokenOptions =>
{
    refreshTokenOptions.ClientId = masaOpenIdConnectOptions.ClientId;
    refreshTokenOptions.ClientSecret = masaOpenIdConnectOptions.ClientSecret;
});

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
//builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();