namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffPaginationQuery(int PageIndex, int PageSize, string SearchKey) : Query<PaginationList<StaffItemDto>>
{

    public override PaginationList<StaffItemDto> Result { get; set; } = null!;
}
