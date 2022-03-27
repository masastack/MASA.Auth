namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffListQuery(string SearchKey, int Count = 20) : Query<List<StaffSelectDto>>
{
    public int MaxCount { get; set; }

    public override List<StaffSelectDto> Result { get; set; } = null!;
}
