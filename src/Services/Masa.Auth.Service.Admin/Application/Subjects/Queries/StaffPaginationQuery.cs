namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffPaginationQuery(int Page, int PageSize, string SearchKey = "", Guid DepartmentId = default(Guid)) : Query<PaginationDto<StaffDto>>
{

    public override PaginationDto<StaffDto> Result { get; set; } = null!;
}
