namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class UpdateApiResourceCommandValidator : AbstractValidator<UpdateApiResourceCommand>
{
    public UpdateApiResourceCommandValidator()
    {
        RuleFor(command => command.ApiResource).SetValidator(new UpdateApiResourceValidator());
    }
}
