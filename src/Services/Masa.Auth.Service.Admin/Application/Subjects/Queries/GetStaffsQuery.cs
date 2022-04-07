namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record GetStaffsQuery(int Page, int PageSize, string Search, bool? Enabled) : Query<PaginationDto<StaffDto>>
{
    public override PaginationDto<StaffDto> Result { get; set; } = new();
}
