using Masa.Auth.Service.Admin.Dto.Permissions;

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleSelectQuery() : Query<List<RoleSelectDto>>
{
    public override List<RoleSelectDto> Result { get; set; } = new();
}
