namespace MASA.Auth.RolePermission.Service.Application.Orders
{
    public class RoleCommandHandler
    {
        private readonly OrderDomainService _domainService;

        public RoleCommandHandler(OrderDomainService domainService)
        {
            _domainService = domainService;
        }

        [EventHandler(Order = 1)]
        public async Task CreateHandleAsync(OrderCreateCommand command)
        {
            await _domainService.PlaceOrderAsync();
            //you work
            await Task.CompletedTask;
        }
    }

    public class OrderStockHandler : CommandHandler<OrderCreateCommand>
    {
        public override Task CancelAsync(OrderCreateCommand comman)
        {
            //cancel todo callback 
            return Task.CompletedTask;
        }

        [EventHandler(FailureLevels = FailureLevels.ThrowAndCancel)]
        public override Task HandleAsync(OrderCreateCommand comman)
        {
            //todo decrease stock
            return Task.CompletedTask;
        }

        [EventHandler(0, FailureLevels.Ignore, IsCancel = true)]
        public Task AddCancelLogs(OrderCreateCommand query)
        {
            //todo increase stock
            return Task.CompletedTask;
        }
    }
}