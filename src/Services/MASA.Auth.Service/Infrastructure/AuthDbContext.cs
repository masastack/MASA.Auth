namespace MASA.Auth.Service.Infrastructure;

public class AuthDbContext : IntegrationEventLogContext
{
    public const string ROLE_SCHEMA = "role";
    public const string SUBJECT_SCHEMA = "subject";
    public const string SSO_SCHEMA = "sso";

    public AuthDbContext(MasaDbContextOptions<AuthDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        base.OnModelCreatingExecuting(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
