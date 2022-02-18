namespace MASA.Auth.UserDepartment.Infrastructure
{
    public class UserDepartmentDbContext : IntegrationEventLogContext
    {
        public const string DEFAULT_SCHEMA = "user";

        public UserDepartmentDbContext(MasaDbContextOptions<UserDepartmentDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreatingExecuting(ModelBuilder builder)
        {
            base.OnModelCreatingExecuting(builder);
        }
    }
}