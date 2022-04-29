namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class AddApiResourceCommandValidator : AbstractValidator<AddApiResourceCommand>
{
    public AddApiResourceCommandValidator()
    {
        RuleFor(command => command.ApiResource).SetValidator(new AddApiResourceValidator());
    }
}
