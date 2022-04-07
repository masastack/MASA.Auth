namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContext : IntegrationEventLogContext
{
    public const string PERMISSION_SCHEMA = "permissions";
    public const string SUBJECT_SCHEMA = "subjects";
    public const string ORGANIZATION_SCHEMA = "organizations";
    public const string SSO_SCHEMA = "sso";

    public AuthDbContext(MasaDbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreatingExecuting(builder);
    }
}
