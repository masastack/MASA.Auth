namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddThirdPartyUserValidator : AbstractValidator<AddThirdPartyUserDto>
{
    public AddThirdPartyUserValidator()
    {
        RuleFor(tpu => tpu.User).SetValidator(new AddUserValidator());
    }
}

