// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
GlobalValidationOptions.SetDefaultCulture("zh-CN");

await builder.Services.AddMasaStackConfigAsync();
var masaStackConfig = builder.Services.GetMasaStackConfig();

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    {
        if (string.IsNullOrEmpty(masaStackConfig.TlsName))
        {
            options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN");
        }
        else
        {
            options.ServerCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
        }
        options.CheckCertificateRevocation = false;
    });
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var authServerUrl = masaStackConfig.GetAuthServiceDomain();

#if DEBUG
authServerUrl = "http://localhost:18002/";
#endif

builder.AddMasaStackComponentsForServer("wwwroot/i18n", authServerUrl);
builder.Services.AddAuthApiGateways(option => option.AuthServiceBaseAddress = authServerUrl);

builder.Services.AddObservable(builder.Logging, () =>
{
    return new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = masaStackConfig.Version,
        ServiceName = masaStackConfig.GetWebId(MasaStackConstant.AUTH)
    };
}, () =>
{
    return masaStackConfig.OtlpUrl;
}, true);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();

builder.Services.AddScoped<IPermissionValidator, PermissionValidator>();
builder.Services.AddSingleton<AddStaffValidator>();
builder.Services.AddTypeAdapter();

MasaOpenIdConnectOptions masaOpenIdConnectOptions = new MasaOpenIdConnectOptions
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackConstant.AUTH),
    Scopes = new List<string> { "offline_access" }
};

IdentityModelEventSource.ShowPII = true;

#if DEBUG
masaOpenIdConnectOptions.Authority = "http://localhost:18200";
#endif

builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions);

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