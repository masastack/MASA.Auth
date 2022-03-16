using Masa.Auth.Service.Admin.Application.Organizations.Models;

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentDetailQuery(Guid DepartmentId) : Query<DepartmentDetail>
{
    public override DepartmentDetail Result { get; set; } = new();
}

