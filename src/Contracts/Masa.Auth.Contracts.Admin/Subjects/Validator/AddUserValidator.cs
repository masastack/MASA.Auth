using Masa.Auth.Contracts.Admin.Infrastructure.Utils;

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddUserValidator : CustomizeAbstractValidator<AddUserDto>
{
    public AddUserValidator()
    {
        RuleFor(user => user.DisplayName).ChineseLetter().MinimumLength(1).MaximumLength(20);
        RuleFor(user => user.Name).ChineseLetter().MaximumLength(20);
        RuleFor(user => user.PhoneNumber).Phone();
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        RuleFor(user => user.CompanyName).ChineseLetter().MinimumLength(1).MaximumLength(50);
        RuleFor(user => user.Position).ChineseLetterNumber().MaximumLength(20);
        RuleFor(user => user.Account).Required().ChineseLetterNumber();
        RuleFor(user => user.Password).Required().LetterNumber();
    }    
}

