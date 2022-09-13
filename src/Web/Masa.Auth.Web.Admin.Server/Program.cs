// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
var publicConfiguration = builder.GetMasaConfiguration().ConfigurationApi.GetPublic();
builder.Services.AddAuthApiGateways(option => option.AuthServiceBaseAddress = publicConfiguration.GetValue<string>("$public.AppSettings:AuthClient:LocalUrl"));
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<IPermissionValidator, PermissionValidator>();
builder.AddMasaStackComponentsForServer("wwwroot/i18n", publicConfiguration.GetValue<string>("$public.AppSettings:AuthClient:LocalUrl"), publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url"));
builder.Services.AddSingleton<AddStaffValidator>();
builder.Services.AddTypeAdapter();
builder.Services.AddMasaOpenIdConnect(publicConfiguration.GetSection("$public.OIDC:AuthClient").Get<MasaOpenIdConnectOptions>());

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