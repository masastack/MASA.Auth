using System.Reflection;

namespace MASA.Auth.RolePermission.Service.Infrastructure
{
    public class RolePermissionDbContext : IntegrationEventLogContext
    {
        public const string DEFAULT_SCHEMA = "role";

        public RolePermissionDbContext(MasaDbContextOptions<RolePermissionDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreatingExecuting(ModelBuilder builder)
        {
            base.OnModelCreatingExecuting(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}