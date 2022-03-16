namespace Masa.Auth.Service.Application.Subjects.Queries;

public record UserPaginationQuery(int PageIndex, int PageSize, string Search,bool Enabled) : Query<PaginationItems<UserItem>>
{
    public override PaginationItems<UserItem> Result { get; set; } = null!;
}
