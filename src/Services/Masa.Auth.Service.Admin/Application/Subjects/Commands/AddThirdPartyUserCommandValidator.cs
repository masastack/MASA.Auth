namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddThirdPartyUserCommandValidator : AbstractValidator<AddThirdPartyUserCommand>
{
    public AddThirdPartyUserCommandValidator()
    {
        RuleFor(command => command.ThirdPartyUser).SetValidator(new AddThirdPartyUserValidator());
    }
}
