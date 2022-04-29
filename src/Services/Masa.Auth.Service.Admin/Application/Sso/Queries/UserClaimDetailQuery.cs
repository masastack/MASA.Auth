namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record UserClaimDetailQuery(int UserClaimId) : Query<UserClaimDetailDto>
{
    public override UserClaimDetailDto Result { get; set; } = new();
}
