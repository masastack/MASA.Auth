// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.ApiGateways.Caller;
using Masa.Auth.Contracts.Admin.Subjects.Validator;
using Masa.Auth.Web.Admin.Rcl;
using Masa.Auth.Web.Admin.Rcl.Global;
using Masa.Auth.Web.Admin.Rcl.Shared;
using Masa.Blazor;
using Masa.Stack.Components;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalForServer();
builder.Services.AddAutoComplete();
builder.Services.AddAuthApiGateways(option => option.AuthServiceBaseAddress = builder.Configuration["AuthServiceBaseAddress"]);

builder.Services.AddScoped<IPermissionValidator, PermissionValidator>();

builder.Services.AddMasaStackComponentsForServer("wwwroot/i18n", builder.Configuration["AuthServiceBaseAddress"], builder.Configuration["McServiceBaseAddress"]);
builder.Services.AddSingleton<AddStaffValidator>();
builder.Services.AddTypeAdapter();
builder.Services.AddMasaOpenIdConnect(builder.Configuration);

builder.WebHost.UseStaticWebAssets();

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