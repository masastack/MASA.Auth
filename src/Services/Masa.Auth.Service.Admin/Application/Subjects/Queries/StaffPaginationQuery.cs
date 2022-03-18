namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffPaginationQuery(int PageIndex, int PageSize, string SearchKey) : Query<PaginationList<StaffDto>>
{

    public override PaginationList<StaffDto> Result { get; set; } = null!;
}
