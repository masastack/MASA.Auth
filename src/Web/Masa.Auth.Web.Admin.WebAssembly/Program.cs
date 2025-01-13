// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton(sp => builder.Configuration);

await builder.Services.AddMasaStackConfigAsync(builder.Configuration);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var masaStackConfig = builder.Services.GetMasaStackConfig();

MasaOpenIdConnectOptions masaOpenIdConnectOptions = new()
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackProject.Auth),
    Scopes = new List<string> { "openid", "profile" }
};

await builder.AddMasaOpenIdConnectAsync(masaOpenIdConnectOptions);

builder.Services.AddAuthApiGateways(option =>
{
    option.AuthServiceBaseAddress = masaStackConfig.GetAuthServiceDomain();
    option.AuthorityEndpoint = masaOpenIdConnectOptions.Authority;
    option.ClientId = masaOpenIdConnectOptions.ClientId;
    option.ClientSecret = masaOpenIdConnectOptions.ClientSecret;
});

builder.Services.AddMasaStackComponent(MasaStackProject.Auth, $"{builder.HostEnvironment.BaseAddress}i18n");

builder.Services.AddScoped<IPermissionValidator, PermissionValidator>();
builder.Services.AddTypeAdapter();


var host = builder.Build();
await host.Services.InitializeMasaStackApplicationAsync();
await host.RunAsync();