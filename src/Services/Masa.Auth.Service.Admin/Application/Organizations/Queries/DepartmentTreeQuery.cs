namespace Masa.Auth.Service.Application.Organizations.Queries;

public record DepartmentTreeQuery(Guid ParentId) : Query<List<DepartmentItem>>
{
    public override List<DepartmentItem> Result { get; set; } = new List<DepartmentItem>();
}

