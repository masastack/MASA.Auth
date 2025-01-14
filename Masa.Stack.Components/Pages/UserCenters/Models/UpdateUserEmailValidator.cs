namespace Masa.Stack.Components.Pages.UserCenters.Models;

internal class UpdateUserEmailValidator : AbstractValidator<UpdateUserEmailModel>
{
    public UpdateUserEmailValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty()
            .WithMessage("email cannot be empty")
            .Matches(@"^\s{0}$|^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage("{PropertyName} format is incorrect");
    }
}
