namespace MASA.Auth.UserDepartment.Infrastructure
{
    public class ShopDbContext : IntegrationEventLogContext
    {
        public DbSet<Order> Orders { get; set; } = default!;

        public ShopDbContext(MasaDbContextOptions<ShopDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreatingExecuting(ModelBuilder builder)
        {
            base.OnModelCreatingExecuting(builder);
        }
    }
}