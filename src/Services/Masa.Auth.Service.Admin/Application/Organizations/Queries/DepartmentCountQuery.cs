namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentCountQuery : Query<DepartmentChildrenCountDto>
{
    public override DepartmentChildrenCountDto Result { get; set; } = new();
}
