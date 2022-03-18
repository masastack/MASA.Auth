using Masa.Auth.Service.Admin.Dto.Subjects;

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserPaginationQuery(int PageIndex, int PageSize, string Search, bool Enabled) : Query<PaginationList<UserDto>>
{
    public override PaginationList<UserDto> Result { get; set; } = null!;
}
