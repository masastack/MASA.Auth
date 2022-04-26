namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record ThirdPartyUserDetailQuery(Guid ThirdPartyUserId) : Query<ThirdPartyUserDetailDto>
{
    public override ThirdPartyUserDetailDto Result { get; set; } = new();
}
