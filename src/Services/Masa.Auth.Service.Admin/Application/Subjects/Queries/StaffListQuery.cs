using Masa.Auth.Service.Admin.Dto.Subjects;

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffListQuery(string SearchKey) : Query<List<StaffItemDto>>
{
    public int MaxCount { get; set; }

    public override List<StaffItemDto> Result { get; set; } = null!;
}
