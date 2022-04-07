namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator() => RuleFor(command => command.User).SetValidator(new UpdateUserValidator());
}
