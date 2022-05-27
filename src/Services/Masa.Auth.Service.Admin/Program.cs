// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprStarter();
builder.Services.AddDaprClient();
builder.Services.AddAliyunStorage();
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

//builder.AddMasaConfiguration(
//configurationBuilder =>
//    {
//        configurationBuilder.UseMasaOptions(options =>
//        {
//            options.Mapping<RedisConfigurationOptions>(SectionTypes.Local, "Appsettings", "RedisConfig");
//            //Map the RedisConfigurationOptions binding to the Local:Appsettings:RedisConfig node
//        });
//    }
//);
MapsterAdapterConfig.TypeAdapter();

builder.Services.AddMasaRedisCache(builder.Configuration.GetSection("RedisConfig"));
builder.Services.AddPmClient(builder.Configuration.GetValue<string>("PmClient:Url"));
builder.Services.AddLadpContext();

builder.Services.AddElasticsearchClient("auth", option => option.UseNodes("http://10.10.90.44:31920/").UseDefault())
                .AddAutoComplete(option => option.UseIndexName("user_index"));

var app = builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
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
            eventBusBuilder.UseMiddleware(typeof(LogMiddleware<>));
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
