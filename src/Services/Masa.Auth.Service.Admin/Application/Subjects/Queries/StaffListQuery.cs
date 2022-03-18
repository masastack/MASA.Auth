namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffListQuery(string SearchKey) : Query<List<StaffDto>>
{
    public int MaxCount { get; set; }

    public override List<StaffDto> Result { get; set; } = null!;
}
