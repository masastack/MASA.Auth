namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user)
            .NotNull().WithMessage($"Parameter error");
        RuleFor(user => user.DisplayName)
                .Matches("^[\u4e00-\u9fa5a-zA-Z-z]+$").WithMessage($"Can only input Chinese and English of {nameof(UpdateUserDto.DisplayName)}")
                .MaximumLength(20).WithMessage($"Please enter 1-20 digits of {nameof(UpdateUserDto.DisplayName)}")
                .MinimumLength(1).WithMessage($"Please enter 1-20 digits of {nameof(UpdateUserDto.DisplayName)}");
        RuleFor(user => user.Name)
                .Matches("^[\u4e00-\u9fa5a-zA-Z-z]+$").WithMessage($"Can only input Chinese and English of {nameof(UpdateUserDto.Name)}")
                .MaximumLength(20).WithMessage($"Please enter 1-20 digits of {nameof(UpdateUserDto.Name)}");
        RuleFor(user => user.Email)
                .Matches("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$").WithMessage($"E-mail format is incorrect");
        RuleFor(user => user.IdCard)
                .Matches("(^\\d{18}$)|(^\\d{15}$)").WithMessage($"IdCard format is incorrect");
    }    
}

