namespace MASA.Auth.UserDepartment.Domain.OrderRepository
{

    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetListAsync();
    }
}