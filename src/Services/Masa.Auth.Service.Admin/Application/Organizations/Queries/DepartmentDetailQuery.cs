namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentDetailQuery(Guid DepartmentId) : Query<DepartmentDetailDto>
{
    public override DepartmentDetailDto Result { get; set; } = new();
}

