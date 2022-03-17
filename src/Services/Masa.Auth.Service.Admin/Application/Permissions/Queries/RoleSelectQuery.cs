namespace Masa.Auth.Service.Application.Permissions.Queries;

public record RoleSelectQuery() : Query<List<RoleSelectItem>>
{
    public override List<RoleSelectItem> Result { get; set; } = new();
}
