namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(command => command.Staff).SetValidator(new AddStaffValidator());
    }
}
