// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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

builder.Services.AddHealthChecks();

builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment);
builder.Services.AddAuthClient(builder.Configuration.GetValue<string>("AuthClient:Url"));
builder.Services.AddPmClient(builder.Configuration.GetValue<string>("PmClient:Url"));

builder.Services.AddOidcCacheStorage(builder.Configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>())
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

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();