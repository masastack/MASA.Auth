// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using EFCoreSecondLevelCacheInterceptor;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();

//if (!builder.Environment.IsProduction())
//{
//    builder.Services.AddDaprStarter(opt =>
//    {
//        opt.DaprHttpPort = 3600;
//        opt.DaprGrpcPort = 3601;
//    });
//}
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

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "";
    options.RequireHttpsMetadata = false;
    options.Audience = "";
});

//builder.AddMasaConfiguration(configurationBuilder =>
//{
//    configurationBuilder.UseDcc();
//    configurationBuilder.UseMasaOptions(option => option.MappingConfigurationApi<IsolationDbConnectionOptions>(""));
//});
MapsterAdapterConfig.TypeAdapter();

builder.Services.AddMasaRedisCache(builder.Configuration.GetSection("RedisConfig")).AddMasaMemoryCache();
builder.Services.AddPmClient(builder.Configuration.GetValue<string>("PmClient:Url"));
builder.Services.AddLadpContext();

builder.Services.AddElasticsearchClient("auth", option => option.UseNodes("http://10.10.90.44:31920/").UseDefault())
                .AddAutoComplete(option => option.UseIndexName("user_index"));

const string providerName1 = "Redis1";
builder.Services.AddEFSecondLevelCache(options =>
                options.UseEasyCachingCoreProvider(providerName1).DisableLogging(false).UseCacheKeyPrefix("EF_")
                .CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30))
                .SkipCachingResults(result =>
                                result.Value == null || (result.Value is EFTableRows rows && rows.RowsCount == 0)));

builder.Services.AddEasyCaching(option =>
{
    option.UseRedis(config =>
    {
        config.DBConfig.AllowAdmin = true;
        config.DBConfig.SyncTimeout = 10000;
        config.DBConfig.AsyncTimeout = 10000;
        config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint("127.0.0.1", 6379));
        config.SerializerName = "mymsgpack";
    }, providerName1).WithMessagePack("mymsgpack");
});


var app = builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
        .UseIsolationUoW<AuthDbContext>(
            isolationBuilder => isolationBuilder.UseMultiEnvironment("env"),
            dbOptions => dbOptions.UseSqlServer().UseFilter())
        .UseRepository<AuthDbContext>();
    })
    .AddServices(builder);

//set Isolation
//app.Use(async (context, next) =>
//{
//    context.Items.Add("env", "development");
//    await next.Invoke();
//});

app.MigrateDbContext<AuthDbContext>((context, services) =>
{
    if (context.Set<Department>().Any())
    {
        return;
    }
    context.Set<Department>().Add(new Department("MasaStack", "MasaStack Root Department"));
    context.SaveChanges();
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

app.Run();
