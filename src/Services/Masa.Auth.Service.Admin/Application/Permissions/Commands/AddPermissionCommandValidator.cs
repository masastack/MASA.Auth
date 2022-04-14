namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public class AddPermissionCommandValidator : AbstractValidator<AddPermissionCommand>
{
    public AddPermissionCommandValidator()
    {
        RuleFor(command => command.PermissionDetail).SetValidator(new PermissionDetailDtoValidator<PermissionDetailDto>());
    }
}
