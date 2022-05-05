namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public record RemoveCustomLoginCommand(RemoveCustomLoginDto CustomLogin) : Command
{
}
