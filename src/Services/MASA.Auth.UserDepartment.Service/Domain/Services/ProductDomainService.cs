using MASA.Auth.UserDepartment.Domain.Events;

namespace MASA.Auth.UserDepartment.Domain.Services
{
    public class ProductDomainService : DomainService
    // you can alse implement an interface like below
    //, ISagaEventHandler<OrderCreatedDomainEvent>
    {

        public ProductDomainService(IDomainEventBus eventBus) : base(eventBus)
        {
        }

        [EventHandler(Order = 1)]
        public void DeductInvenroyCompletedAsync(OrderCreatedDomainEvent @event)
        {
            //todo after decrease stock,like Pub Event to other micro service
        }

        [EventHandler(Order = 0, FailureLevels = FailureLevels.ThrowAndCancel)]
        public Task DeductInvenroyAsync(OrderCreatedDomainEvent @event)
        {
            //todo decrease stock
            throw new NotImplementedException();
        }

        [EventHandler(1, FailureLevels.Ignore, IsCancel = true)]
        public Task CancelDeductInvenroyAsync(OrderCreatedDomainEvent @event)
        {
            //throw exception,todo increase stock
            throw new NotImplementedException();
        }

    }
}
