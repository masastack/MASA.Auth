namespace MASA.Auth.RolePermission.Service.Application.Orders.Commands
{
    public class OrderCreateCommandValidator : AbstractValidator<OrderCreateCommand>
    {
        public OrderCreateCommandValidator()
        {
            RuleFor(cmd => cmd.Items).Must(cmd => cmd.Any()).WithMessage("the order items cannot be empty");
        }
    }
}