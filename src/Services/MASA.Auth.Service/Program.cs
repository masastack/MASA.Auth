var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
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

builder.AddMasaConfiguration(
configurationBuilder =>
{
    configurationBuilder.UseMasaOptions(options =>
    {
        options.Mapping<RedisConfigurationOptions>(SectionTypes.Local, "Appsettings", "RedisConfig");
        //Map the RedisConfigurationOptions binding to the Local:Appsettings:RedisConfig node
    });
});

var serviceProvider = builder.Services.BuildServiceProvider()!;
var redisOptions = serviceProvider.GetService<IOptions<RedisConfigurationOptions>>();
builder.Services.AddMasaRedisCache(redisOptions!.Value);

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
    .AddTransient(typeof(IMiddleware<>), typeof(LogMiddleware<>))
    .AddTransient(typeof(IMiddleware<>), typeof(ValidatorMiddleware<>))
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssemblyContaining<Program>();
    })
    .AddDomainEventBus(options =>
    {
        options.UseEventBus()
               .UseUoW<AuthDbContext>(dbOptions =>
                    //dbOptions.UseSqlServer("server=masa.auth.database;uid=sa;pwd=P@ssw0rd;database=masa_auth")
                    dbOptions.UseSqlServer("server=.;uid=sa;pwd=P@ssw0rd;database=masa_auth")
               )
               .UseDaprEventBus<IntegrationEventLogService>()
               .UseEventLog<AuthDbContext>()
               .UseRepository<AuthDbContext>();
    })
    .AddServices(builder);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
