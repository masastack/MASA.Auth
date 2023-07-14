// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
GlobalValidationOptions.SetDefaultCulture("zh-CN");

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.Auth, MasaStackApp.WEB);
var masaStackConfig = builder.Services.GetMasaStackConfig();

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseKestrel(option =>
    {
        option.ConfigureHttpsDefaults(options =>
        {
            options.ServerCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
            options.CheckCertificateRevocation = false;
        });
    });
}

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(option =>
{
    if (builder.Environment.IsDevelopment())
    {
        option.DetailedErrors = true;
    }
});

MasaOpenIdConnectOptions masaOpenIdConnectOptions = new()
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackProject.Auth),
    Scopes = new List<string> { "offline_access" }
};

IdentityModelEventSource.ShowPII = true;

var authServerUrl = masaStackConfig.GetAuthServiceDomain();

#if DEBUG
authServerUrl = "http://localhost:18002/";
masaOpenIdConnectOptions.Authority = "http://localhost:18200";
#endif

builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions);

builder.Services.AddAuthApiGateways(option =>
{
    option.AuthServiceBaseAddress = authServerUrl;
    option.AuthorityEndpoint = masaOpenIdConnectOptions.Authority;
    option.ClientId = masaOpenIdConnectOptions.ClientId;
    option.ClientSecret = masaOpenIdConnectOptions.ClientSecret;
});

await builder.Services.AddMasaStackComponentsAsync(MasaStackProject.Auth, "wwwroot/i18n", authServerUrl);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();

builder.Services.AddScoped<IPermissionValidator, PermissionValidator>();
builder.Services.AddTypeAdapter();

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