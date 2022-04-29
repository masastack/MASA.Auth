namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public record RemoveUserClaimCommand(RemoveUserClaimDto UserClaim) : Command
{
}
