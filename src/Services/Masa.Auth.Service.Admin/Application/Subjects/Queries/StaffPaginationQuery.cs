namespace Masa.Auth.Service.Application.Subjects.Queries;

public record StaffPaginationQuery(int PageIndex, int PageSize, string SearchKey) : Query<PaginationItems<StaffItem>>
{

    public override PaginationItems<StaffItem> Result { get; set; } = null!;
}
