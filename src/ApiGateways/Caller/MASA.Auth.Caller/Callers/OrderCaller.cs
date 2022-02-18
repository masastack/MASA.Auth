namespace MASA.Auth.Caller.Callers
{
    public class OrderCaller : HttpClientCallerBase
    {
        protected override string BaseAddress { get; set; } = "http://localhost:6102";

        public OrderCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = nameof(OrderCaller);
        }

        public async Task<List<Order>> GetListAsync()
        {
            return await CallerProvider.GetAsync<List<Order>>($"order/list");
        }
    }
}