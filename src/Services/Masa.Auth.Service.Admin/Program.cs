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

builder.Services.AddDaprClient();
builder.Services.AddAliyunStorage(serviceProvider =>
{
    var daprClient = serviceProvider.GetRequiredService<DaprClient>();
    var accessId = daprClient.GetSecretAsync("localsecretstore", "access_id").Result.First().Value;
    var accessSecret = daprClient.GetSecretAsync("localsecretstore", "access_secret").Result.First().Value;
    var endpoint = daprClient.GetSecretAsync("localsecretstore", "endpoint").Result.First().Value;
    var roleArn = daprClient.GetSecretAsync("localsecretstore", "roleArn").Result.First().Value;
    return new AliyunStorageOptions(accessId, accessSecret, endpoint, roleArn, "SessionTest")
    {
        Sts = new AliyunStsOptions()
        {
            RegionId = "cn-hangzhou"
        }
    };
});

builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = builder.Configuration["ConfigurationAPI:Masa_Auth_Web:AppSettings:IdentityServerUrl"];
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
var redisConfigOption = builder.Configuration.GetSection("ConfigurationAPI:Masa_Auth_Web:AppSettings:RedisConfig").Get<RedisConfigurationOptions>();
builder.Services.AddMasaRedisCache(redisConfigOption).AddMasaMemoryCache();
builder.Services.AddPmClient(builder.Configuration.GetValue<string>("ConfigurationAPI:Masa_Auth_Web:AppSettings:PmClient:Url"));
builder.Services.AddLadpContext();

builder.Services.AddElasticsearchAutoComplete(builder.Configuration.GetSection("ConfigurationAPI:Masa_Auth_Web:AppSettings:AutoComplete"));

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
    .UseDaprEventBus<IntegrationEventLogService>(options => options.UseEventLog<AuthDbContext>())
    .UseEventBus(eventBusBuilder =>
    {
        eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
    })
    //set Isolation.
    //this project is physical isolation,logical isolation AggregateRoot(Entity) neet to implement interface IMultiEnvironment
    .UseIsolationUoW<AuthDbContext>(
        isolationBuilder => isolationBuilder.UseMultiEnvironment(IsolationConsts.ENVIRONMENT_KEY),
        dbOptions => dbOptions.UseSqlServer().UseFilter())
    .UseRepository<AuthDbContext>();
});

builder.Services.AddOidcCache(redisConfigOption);
await builder.Services.AddOidcDbContext<AuthDbContext>(async option =>
{
    await option.SeedStandardResourcesAsync();
    await option.SeedClientDataAsync(new List<Client>
    {
        builder.Configuration.GetSection("ConfigurationAPI:Masa_Auth_Web:AppSettings:Client").Get<ClientModel>().Adapt<Client>()
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

app.UseMasaExceptionHandling(opt =>
{
    opt.CustomExceptionHandler = exception =>
    {
        Exception friendlyException = exception;
        if (exception is ValidationException validationException)
        {
            friendlyException = new UserFriendlyException(validationException.Errors.Select(err => err.ToString()).FirstOrDefault()!);
        }
        return (friendlyException, false);
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
