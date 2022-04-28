namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public class UpsertPositionCommandCommandValidator : AbstractValidator<UpsertPositionCommand>
{
    public UpsertPositionCommandCommandValidator()
    {
        RuleFor(command => command.Position).SetValidator(new UpsertPositionValidator());
    }
}
