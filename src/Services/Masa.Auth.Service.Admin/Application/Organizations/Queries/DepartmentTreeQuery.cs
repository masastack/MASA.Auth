using Masa.Auth.Service.Admin.Dto.Organizations;

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentTreeQuery(Guid ParentId) : Query<List<DepartmentDto>>
{
    public override List<DepartmentDto> Result { get; set; } = new List<DepartmentDto>();
}

