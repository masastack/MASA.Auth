namespace MASA.Auth.Service.Application.Subjects.Queries;

public record StaffListQuery(string SearchKey) : Query<List<StaffItem>>
{
    public int MaxCount { get; set; }

    public override List<StaffItem> Result { get; set; } = null!;
}
