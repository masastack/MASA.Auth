using MASA.Auth.UserDepartment.Domain.Aggregate.Orders;
using MASA.Auth.UserDepartment.Domain.Services;

namespace MASA.Auth.UserDepartment.Services
{
    public class OrderService : ServiceBase
    {
        public OrderService(IServiceCollection services) : base(services)
        {
            App.MapGet("/order/list", QueryList).Produces<List<Order>>()
                .WithName("GetOrders")
                .RequireAuthorization();
            App.MapPost("/placeOrder", PlaceOrder);
        }


        public async Task<IResult> QueryList(OrderDomainService orderDomainService)
        {
            var orders = await orderDomainService.QueryListAsync();
            return Results.Ok(orders);
        }

        public async Task<IResult> PlaceOrder(IEventBus eventBus)
        {
            var comman = new OrderCreateCommand();
            await eventBus.PublishAsync(comman);
            return Results.Ok();
        }
    }
}