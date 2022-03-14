namespace Masa.Auth.Service.Application.Subjects.Queries;

public record UserPaginationQuery(int PageIndex, int PageSize, string Search,bool Enabled) : Query<PaginationList<UserItem>>
{
    public override PaginationList<UserItem> Result { get; set; } = null!;
}
