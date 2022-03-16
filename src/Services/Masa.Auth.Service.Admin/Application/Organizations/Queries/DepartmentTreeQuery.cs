using Masa.Auth.Service.Admin.Application.Organizations.Models;

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentTreeQuery(Guid ParentId) : Query<List<DepartmentItem>>
{
    public override List<DepartmentItem> Result { get; set; } = new List<DepartmentItem>();
}

