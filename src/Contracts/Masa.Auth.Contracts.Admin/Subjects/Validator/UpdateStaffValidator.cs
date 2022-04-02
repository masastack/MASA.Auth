namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateStaffValidator : AbstractValidator<UpdateStaffDto>
{
    public UpdateStaffValidator()
    {
        RuleFor(staff => staff.User).SetValidator(new UpdateUserValidator());
    }
}

