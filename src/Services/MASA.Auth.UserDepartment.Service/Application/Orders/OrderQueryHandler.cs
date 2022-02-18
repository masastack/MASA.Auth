using MASA.Auth.UserDepartment.Application.Orders.Queries;
using MASA.Auth.UserDepartment.Domain.OrderRepository;

namespace MASA.Auth.UserDepartment.Application.Orders
{
    public class OrderQueryHandler
    {
        readonly IOrderRepository _orderRepository;
        public OrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [EventHandler]
        public async Task OrderListHandleAsync(OrderQuery query)
        {
            query.Result = await _orderRepository.GetListAsync();
        }
    }
}