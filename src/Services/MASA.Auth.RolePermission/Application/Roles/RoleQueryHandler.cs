namespace MASA.Auth.RolePermission.Service.Application.Orders
{
    public class RoleQueryHandler
    {
        readonly IOrderRepository _orderRepository;
        public RoleQueryHandler(IOrderRepository orderRepository)
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