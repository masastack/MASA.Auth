namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentCountQuery : Query<DepartmentCount>
{
    public override DepartmentCount Result { get; set; } = new();
}
