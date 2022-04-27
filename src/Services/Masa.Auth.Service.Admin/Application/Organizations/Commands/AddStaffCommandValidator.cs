namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public class AddOrUpdatePositionCommandValidator : AbstractValidator<AddOrUpdatePositionCommand>
{
    public AddOrUpdatePositionCommandValidator()
    {
        RuleFor(command => command.Position).SetValidator(new AddOrUpdatePositionValidator());
    }
}
