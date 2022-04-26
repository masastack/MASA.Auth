namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record ThirdPartyUsersQuery(int Page, int PageSize, Guid UserId, bool? Enabled,DateTime? StartTime, DateTime? EndTime) : Query<PaginationDto<ThirdPartyUserDto>>
{
    public override PaginationDto<ThirdPartyUserDto> Result { get; set; } = new();
}
