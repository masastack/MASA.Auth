namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffSelectQuery(string Search) : Query<List<StaffSelectDto>>
{
    public int MaxCount { get; set; }

    public override List<StaffSelectDto> Result { get; set; } = new();
}
