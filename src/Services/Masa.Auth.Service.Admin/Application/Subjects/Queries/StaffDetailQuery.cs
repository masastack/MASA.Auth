namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffDetailQuery(Guid StaffId) : Query<StaffDetailDto>
{
    public override StaffDetailDto Result { get; set; } = new();
}
