namespace Masa.Auth.Service.Application.Subjects.Queries;

public record StaffPaginationQuery(int PageIndex, int PageSize, string SearchKey) : Query<PaginationList<StaffItem>>
{

    public override PaginationList<StaffItem> Result { get; set; } = null!;
}
