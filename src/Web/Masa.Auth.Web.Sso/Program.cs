// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoInject();

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.Auth, MasaStackApp.SSO);
var masaStackConfig = builder.Services.GetMasaStackConfig();

// Add services to the container.
builder.Services.AddSingleton<EnvironmentDataService>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor(builder =>
{
    builder.ConfigureTheme(theme =>
    {
        theme.Dark = false;
        theme.Themes.Light.Primary = "#4318FF";
        theme.Themes.Light.Accent = "#4318FF";
    });
}).AddI18nForServer("wwwroot/i18n");
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();
builder.Services.AddMasaIdentity();

#if DEBUG
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
#endif
StackExchangeRedisInstrumentation redisInstrumentation = default!;
var authDomain = masaStackConfig.GetAuthServiceDomain();

#if DEBUG
authDomain = "http://localhost:18002";
#endif

//tsc
builder.Services.AddObservable(builder.Logging, new MasaObservableOptions
{
    ServiceNameSpace = builder.Environment.EnvironmentName,
    ServiceVersion = masaStackConfig.Version,
    ServiceName = masaStackConfig.GetId(MasaStackProject.Auth, MasaStackApp.SSO),
    Layer = masaStackConfig.Namespace,
    ServiceInstanceId = builder.Configuration.GetValue<string>("HOSTNAME")!
}, masaStackConfig.OtlpUrl, true
, traceInstrumentConfig: traceBuilder =>
{
    traceBuilder.AddRedisInstrumentation(options =>
            {
                options.SetVerboseDatabaseStatements = true;
            })
            .ConfigureRedisInstrumentation(instrumentation =>
            {
                redisInstrumentation = instrumentation;
            });
});


var redisOption = new RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions> {
        new RedisServerOptions()
        {
            Host= masaStackConfig.RedisModel.RedisHost,
            Port= masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword
};

builder.Services.AddAuthClient(authDomain, redisOption, connectConfig: connect => redisInstrumentation.AddConnection(connect));
builder.Services.AddMcClient(masaStackConfig.GetMcServiceDomain());
builder.Services.AddPmClient(masaStackConfig.GetPmServiceDomain());
builder.Services.AddTransient<IConsentMessageStore, ConsentResponseStore>();
builder.Services.AddScoped<IEventSink, IdentityServerEventSink>();
builder.Services.AddSameSiteCookiePolicy();
builder.Services.AddMultilevelCache(distributedCacheOptions =>
{
    distributedCacheOptions.UseStackExchangeRedisCache(redisOption, connectConfig: connect => redisInstrumentation.AddConnection(connect));
});
builder.Services.AddScoped<CookieStorage>();

var connectionString = masaStackConfig.GetConnectionString(MasaStackProject.Auth.Name);

builder.Services.AddDbContext<PersistedGrantDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var identityServerBuilder = builder.Services.AddOidcCacheStorage(redisOption)
    .AddIdentityServer(options =>
    {
        options.UserInteraction.ErrorUrl = "/error/500";
        options.Events.RaiseSuccessEvents = true;
    })
    .AddOperationalStore<PersistedGrantDbContext>(options =>
    {
        var migrationsAssembly = typeof(PersistedGrantDbContext).Assembly.GetName().Name;
        options.ConfigureDbContext = builder =>
            builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

        options.EnableTokenCleanup = true;
        // IS4 默认表名是复数形式，覆盖为与 AuthDbContext 一致的 auth schema 单数表名
        options.PersistedGrants = new TableConfiguration("PersistedGrant", "auth");
        options.DeviceFlowCodes = new TableConfiguration("DeviceCodes", "auth");
    })
    .AddClientStore<MasaClientStore>()
    .AddResourceStore<MasaResourceStore>()
    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddProfileService<UserProfileService>()
    .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
    .AddExtensionGrantValidator<PhoneCodeGrantValidator>()
    .AddExtensionGrantValidator<LocalPhoneNumberGrantValidator>()
    .AddExtensionGrantValidator<ThirdPartyIdpGrantValidator>()
    .AddExtensionGrantValidator<LdapGrantValidator>()
    .AddExtensionGrantValidator<ImpersonationGrantValidator>();

if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();
}
else
{
    var serverCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
    builder.WebHost.UseKestrel(option =>
    {
        option.ConfigureHttpsDefaults(options =>
        {
            options.ServerCertificate = serverCertificate;
            options.CheckCertificateRevocation = false;
        });
    });
    identityServerBuilder.AddSigningCredential(serverCertificate);
}


var protectionConnect = ConnectionMultiplexer.Connect((ConfigurationOptions)redisOption);
redisInstrumentation.AddConnection(protectionConnect);
builder.Services.AddSingleton<IConnectionMultiplexer>(protectionConnect);
builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(protectionConnect);

builder.Services.AddHotUpdateAuthenticationExternal<AuthenticationExternalHandler, RemoteAuthenticationDefaultsProvider>();

builder.Services.AddScoped<IUserSession, ClientUserSession>();

builder.Services.AddSingleton<SsoAuthenticationStateCache>();
builder.Services.AddScoped<AuthenticationStateProvider, SsoAuthenticationStateProvider>();
builder.Services.AddLadpContext();

builder.Services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());

builder.Services.PostConfigure<CookieAuthenticationOptions>(
    IdentityServerConstants.DefaultCookieAuthenticationScheme, options =>
    {
        var original = options.Events.OnValidatePrincipal;
        options.Events.OnValidatePrincipal = async context =>
        {
            if (original != null)
                await original(context);

            var sub = context.Principal?.FindFirst("sub")?.Value;
            if (sub != null)
            {
                var mux = context.HttpContext.RequestServices
                    .GetRequiredService<IConnectionMultiplexer>();
                var db = mux.GetDatabase();
                if (await db.KeyExistsAsync($"kicked:{sub}"))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(
                        IdentityServerConstants.DefaultCookieAuthenticationScheme);
                    await db.KeyDeleteAsync($"kicked:{sub}");
                }
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

//todo https://github.com/jsakamoto/Toolbelt.Blazor.HeadElement
app.Use(async (context, next) =>
{
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
    if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    }

    //TODO
    //1.Remove X-Frame-Options header
    //2.Control allowed parent pages via CSP frame-ancestors
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
    if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
    {
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    }
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
    var csp = "default-src * 'self'; object-src 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";//frame-ancestors 'none'; 
    // also consider adding upgrade-insecure-requests once you have HTTPS in place for production
    csp += "upgrade-insecure-requests;";
    csp += "img-src * 'self' data:; script-src 'self' 'unsafe-inline' 'unsafe-eval' * data:;style-src  'self' 'unsafe-inline' *;frame-src *";

    // once for standards compliant browsers
    if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
    {
        context.Response.Headers.Add("Content-Security-Policy", csp);
    }
    // and once again for IE
    if (!context.Response.Headers.ContainsKey("X-Content-Security-Policy"))
    {
        context.Response.Headers.Add("X-Content-Security-Policy", csp);
    }

    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
    var referrer_policy = "no-referrer";
    if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
    {
        context.Response.Headers.Add("Referrer-Policy", referrer_policy);
    }
    await next();
});

app.UseStaticFiles();
app.UseRouting();

#if DEBUG
app.UseCors("AllowAllOrigins");
#endif

app.UseIdentityServer();
// This cookie policy fixes login issues with Chrome 80+ using HHTP
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseAuthentication()
   .UseAuthorizationExternal();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
