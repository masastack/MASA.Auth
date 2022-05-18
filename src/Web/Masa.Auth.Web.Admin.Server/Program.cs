// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.ApiGateways.Caller;
using Masa.Auth.Contracts.Admin.Subjects.Validator;
using Masa.Auth.Web.Admin.Rcl;
using Masa.Auth.Web.Admin.Rcl.Global;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.IdentityModel.Tokens;

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
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
builder.Services.AddGlobalForServer();
builder.Services.AddAutoComplete();
builder.Services.AddAuthApiGateways(option => option.AuthServiceBaseAddress = builder.Configuration["AuthServiceBaseAddress"]);
builder.Services.AddSingleton<AddStaffValidator>();
builder.Services.AddTypeAdapter();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
                   options =>
                   {
                       options.RequireHttpsMetadata = false;

                       options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                       options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                       // Set Authority to setting in appsettings.json.  This is the URL of the IdentityServer4
                       options.Authority = "OIDC:Authority";
                       // Set ClientId to setting in appsettings.json.    This Client ID is set when registering the Blazor Server app in IdentityServer4
                       options.ClientId = "OIDC:ClientId";
                       // Set ClientSecret to setting in appsettings.json.  The secret value is set from the Client >  Basic tab in IdentityServer Admin UI
                       options.ClientSecret = "OIDC:ClientSecret";
                       // When set to code, the middleware will use PKCE protection
                       options.ResponseType = "id_token token";
                       // Add request scopes.  The scopes are set in the Client >  Basic tab in IdentityServer Admin UI
                       options.Scope.Add("openid");
                       options.Scope.Add("profile");
                       options.Scope.Add("email");
                       // Save access and refresh tokens to authentication cookie.  the default is false
                       options.SaveTokens = true;
                       // It's recommended to always get claims from the 
                       // UserInfoEndpoint during the flow. 
                       options.GetClaimsFromUserInfoEndpoint = true;
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           //map claim to name for display on the upper right corner after login.  Can be name, email, etc.
                           NameClaimType = "name"
                       };

                       options.Events = new OpenIdConnectEvents
                       {
                           OnAccessDenied = context =>
                           {
                               context.HandleResponse();
                               context.Response.Redirect("/");
                               return Task.CompletedTask;
                           }
                       };
                   });

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();