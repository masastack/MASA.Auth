namespace MASA.Auth.Service.Application.Organizations.Queries;

public record DepartmentTreeQuery : Query<List<DepartmentItem>>
{
    public override List<DepartmentItem> Result { get; set; } = new List<DepartmentItem>();
}

