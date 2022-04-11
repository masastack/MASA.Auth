namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record ApiPermissionSelectQuery : Query<SelectItemDto<Guid>>
{
    public override SelectItemDto<Guid> Result { get; set; } = new();
}
