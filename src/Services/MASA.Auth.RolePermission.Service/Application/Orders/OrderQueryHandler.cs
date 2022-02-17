namespace MASA.Auth.RolePermission.Service.Application.Orders
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