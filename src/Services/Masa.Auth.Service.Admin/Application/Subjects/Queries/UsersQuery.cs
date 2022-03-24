namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UsersQuery(int Page, int PageSize, Guid userId, bool Enabled) : Query<PaginationDto<UserDto>>
{
    public override PaginationDto<UserDto> Result { get; set; } = new();
}
