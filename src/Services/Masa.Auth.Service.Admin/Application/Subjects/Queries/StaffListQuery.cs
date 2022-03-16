using Masa.Auth.Service.Admin.Application.Subjects.Models;

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffListQuery(string SearchKey) : Query<List<StaffItem>>
{
    public int MaxCount { get; set; }

    public override List<StaffItem> Result { get; set; } = null!;
}
