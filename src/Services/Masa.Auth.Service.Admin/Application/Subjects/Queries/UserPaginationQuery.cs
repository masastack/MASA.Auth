using Masa.Auth.Service.Admin.Application.Subjects.Models;
using Masa.Auth.Service.Admin.Infrastructure.Models;

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserPaginationQuery(int PageIndex, int PageSize, string Search, bool Enabled) : Query<PaginationList<UserItem>>
{
    public override PaginationList<UserItem> Result { get; set; } = null!;
}
