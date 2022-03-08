namespace Masa.Auth.Service.Application.Organizations.Queries;

public record DepartmentTreeQuery(string Name, Guid ParentId) : Query<List<DepartmentItem>>
{
    public override List<DepartmentItem> Result { get; set; } = new List<DepartmentItem>();
}

