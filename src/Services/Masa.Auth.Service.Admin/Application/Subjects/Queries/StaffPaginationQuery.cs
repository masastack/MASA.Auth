namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffPaginationQuery(int PageIndex, int PageSize, Guid DepartmentId, string SearchKey) : Query<PaginationList<StaffItem>>
{

    public override PaginationList<StaffItem> Result { get; set; } = null!;
}
