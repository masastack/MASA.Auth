namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserPaginationQuery(int Page, int PageSize, string Search, bool Enabled) : Query<PaginationDto<UserDto>>
{
    public override PaginationDto<UserDto> Result { get; set; } = new();
}
