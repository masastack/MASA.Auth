namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(command => command.Staff).SetValidator(new UpdateStaffValidator());
    }
}
