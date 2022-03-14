namespace Masa.Auth.Service.Application.Organizations.Queries;

public record DepartmentDetailQuery(Guid DepartmentId) : Query<DepartmentDetail>
{
    public override DepartmentDetail Result { get; set; } = new();
}

