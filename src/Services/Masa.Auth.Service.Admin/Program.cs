// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability();

builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
await new DccSeed().SeedAsync(builder);

#if DEBUG
builder.Services.AddDaprStarter(opt =>
{
    opt.DaprHttpPort = 3600;
    opt.DaprGrpcPort = 3601;
});
#endif

builder.Services.AddAutoInject();
builder.Services.AddDaprClient();

var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();
var ossOptions = publicConfiguration.GetSection("$public.OSS").Get<OssOptions>();
builder.Services.AddAliyunStorage(new AliyunStorageOptions(ossOptions.AccessId, ossOptions.AccessSecret, ossOptions.Endpoint, ossOptions.RoleArn, ossOptions.RoleSessionName)
{
    Sts = new AliyunStsOptions()
    {
        RegionId = ossOptions.RegionId
    }
});

builder.Services.AddMasaIdentity(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
    options.Mapping(nameof(MasaUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
    options.Mapping(nameof(MasaUser.StaffId), IdentityClaimConsts.STAFF);
    options.Mapping(nameof(MasaUser.Account), IdentityClaimConsts.ACCOUNT);
});
builder.Services
    .AddScoped<EnvironmentMiddleware>()
    .AddScoped<IAuthorizationMiddlewareResultHandler, CodeAuthorizationMiddlewareResultHandler>()
    .AddSingleton<IAuthorizationHandler, DefaultRuleCodeAuthorizationHandler>()
    .AddSingleton<IAuthorizationPolicyProvider, DefaultRuleCodePolicyProvider>()
    .AddAuthorization(options =>
    {
        var unexpiredPolicy = new AuthorizationPolicyBuilder()
            // Remove if you don't need the user to be authenticated
            .RequireAuthenticatedUser()
            .AddRequirements(new DefaultRuleCodeRequirement(MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID))
            .Build();
        options.DefaultPolicy = unexpiredPolicy;
    })
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        //todo dcc
        options.Authority = builder.Services.GetMasaConfiguration().Local.GetValue<string>("IdentityServerUrl");
        options.RequireHttpsMetadata = false;
        //options.Audience = "";
        options.TokenValidationParameters.ValidateAudience = false;
        options.MapInboundClaims = false;
    });

MapsterAdapterConfig.TypeAdapter();

builder.Services.AddDccClient();

var redisOption = publicConfiguration.GetSection("$public.RedisConfig").Get<RedisConfigurationOptions>();
builder.Services.AddMultilevelCache(options => options.UseStackExchangeRedisCache(redisOption));
builder.Services.AddAuthClientMultilevelCache(redisOption);

await builder.Services
            .AddPmClient(publicConfiguration.GetValue<string>("$public.AppSettings:PmClient:Url"))
            .AddSchedulerClient(publicConfiguration.GetValue<string>("$public.AppSettings:SchedulerClient:Url"))
            .AddMcClient(publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url"))
            .AddLadpContext()
            .AddElasticsearchAutoComplete()
            .AddSchedulerJobAsync();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("A healthy result."))
    .AddDbContextCheck<AuthDbContext>();
builder.Services
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
.AddEndpointsApiExplorer()
.AddSwaggerGen(options =>
{
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xxxxxxxxxxxxxxx\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
})
.AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Program>();
})
.AddDomainEventBus(dispatcherOptions =>
{
    dispatcherOptions
    .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<AuthDbContext>())
    .UseEventBus(eventBusBuilder =>
    {
        eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
        eventBusBuilder.UseMiddleware(typeof(DisabledCommandMiddleware<>));
    })
    //set Isolation.
    //this project is physical isolation,logical isolation AggregateRoot(Entity) neet to implement interface IMultiEnvironment
    .UseIsolationUoW<AuthDbContext>(
        isolationBuilder => isolationBuilder.UseMultiEnvironment(IsolationConsts.ENVIRONMENT),
        dbOptions => dbOptions.UseSqlServer().UseFilter())
    .UseRepository<AuthDbContext>();
});

var defaultConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault();
builder.Services.AddOidcCache(defaultConfiguration);
await builder.Services.AddOidcDbContext<AuthDbContext>(async option =>
{
    await option.SeedStandardResourcesAsync();
    await option.SeedClientDataAsync(new List<Client>
    {
        defaultConfiguration.GetSection("ClientSeed").Get<ClientModel>().Adapt<Client>()
    });
    await option.SyncCacheAsync();
});
builder.Services.RemoveAll(typeof(IProcessor));

var app = builder.AddServices(options =>
{
    options.DisableAutoMapRoute = true; // todo :remove it before v1.0
    options.GetPrefixes = new() { "Get", "Select", "Find" };
    options.PostPrefixes = new() { "Post", "Add", "Create", "Send" };
});

app.MigrateDbContext<AuthDbContext>((context, services) =>
{
    var logger = services.GetRequiredService<ILogger<AuthDbContextSeed>>();
    new AuthDbContextSeed().SeedAsync(context, logger).Wait();
});
app.UseMasaExceptionHandler(opt =>
{
    opt.ExceptionHandler = context =>
    {
        if (context.Exception is ValidationException validationException)
        {
            context.ToResult(validationException.Errors.Select(err => err.ToString()).FirstOrDefault()!);
        }
    };
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<EnvironmentMiddleware>();

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
});
app.UseHttpsRedirection();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.Run();
