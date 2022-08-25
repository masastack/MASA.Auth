// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();

#if DEBUG
builder.Services.AddDaprStarter(opt =>
{
    opt.DaprHttpPort = 3600;
    opt.DaprGrpcPort = 3601;
});
#endif

builder.Services.AddAutoInject();
builder.Services.AddDaprClient();
builder.Services.AddAliyunStorage(async serviceProvider =>
{
    var daprClient = serviceProvider.GetRequiredService<DaprClient>();
    var aliyunOssConfig = await daprClient.GetSecretAsync("localsecretstore", "aliyun-oss");
    var accessId = aliyunOssConfig["access_id"];
    var accessSecret = aliyunOssConfig["access_secret"];
    var endpoint = aliyunOssConfig["endpoint"];
    var roleArn = aliyunOssConfig["role_arn"];
    return new AliyunStorageOptions(accessId, accessSecret, endpoint, roleArn, "SessionTest")
    {
        Sts = new AliyunStsOptions()
        {
            RegionId = "cn-hangzhou"
        }
    };
});

builder.Services.AddMasaIdentityModel(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});

builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, CodeAuthorizationMiddlewareResultHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultRuleCodePolicyProvider>();
builder.Services.AddAuthorization(options =>
{
    var unexpiredPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser() // Remove if you don't need the user to be authenticated
        .AddRequirements(new DefaultRuleCodeRequirement(MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID))
        .Build();
    options.DefaultPolicy = unexpiredPolicy;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    //todo dcc
    options.Authority = builder.GetMasaConfiguration().Local.GetValue<string>("IdentityServerUrl");
    options.RequireHttpsMetadata = false;
    //options.Audience = "";
    options.TokenValidationParameters.ValidateAudience = false;
    options.MapInboundClaims = false;
});

MapsterAdapterConfig.TypeAdapter();

builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
builder.Services.AddDccClient();
var configuration = builder.GetMasaConfiguration().ConfigurationApi.GetDefault();
builder.Services.AddMasaRedisCache(configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>());
builder.Services.AddPmClient(configuration.GetValue<string>("AppSettings:PmClient:Url"));
builder.Services.AddSchedulerClient(configuration.GetValue<string>("AppSettings:SchedulerClient:Url"));
await builder.Services.AddSchedulerJobAsync();
builder.Services.AddMcClient(configuration.GetValue<string>("AppSettings:McClient:Url"));
builder.Services.AddLadpContext();
builder.Services.AddElasticsearchAutoComplete();
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
    })
    //set Isolation.
    //this project is physical isolation,logical isolation AggregateRoot(Entity) neet to implement interface IMultiEnvironment
    .UseIsolationUoW<AuthDbContext>(
        isolationBuilder => isolationBuilder.UseMultiEnvironment(IsolationConsts.ENVIRONMENT),
        dbOptions => dbOptions.UseSqlServer().UseFilter())
    .UseRepository<AuthDbContext>();
});

builder.Services.AddOidcCache(configuration);
await builder.Services.AddOidcDbContext<AuthDbContext>(async option =>
{
    await option.SeedStandardResourcesAsync();
    await option.SeedClientDataAsync(new List<Client>
    {
        builder.GetMasaConfiguration().ConfigurationApi.GetDefault().GetSection("ClientSeed").Get<ClientModel>().Adapt<Client>()
    });
    await option.SyncCacheAsync();
});
builder.Services.RemoveAll(typeof(IProcessor));

var app = builder.Services.AddServices(builder);

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
app.UseMiddleware<MasaAuthorizeMiddleware>();

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
