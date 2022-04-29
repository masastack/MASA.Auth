namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record UserClaimSelectQuery(string? Search) : Query<List<UserClaimSelectDto>>
{
    public override List<UserClaimSelectDto> Result { get; set; } = new();
}
