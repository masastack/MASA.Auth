namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddTeamCommandValidator : AbstractValidator<AddTeamCommand>
{
    public AddTeamCommandValidator()
    {
        RuleFor(command => command).NotNull().WithMessage($"Parameter error");
        RuleFor(command => command.AddTeamDto.Name).Must(name => !string.IsNullOrEmpty(name) && name.Length <= 20)
            .WithMessage("Team Name can`t null and length must be less than 20.");
        RuleFor(command => command.AddTeamDto.Description).Must(description => description.Length <= 255)
            .WithMessage("Team Description length must be less than 255.");
    }
}
