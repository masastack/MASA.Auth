namespace Masa.Auth.Service.Admin.Dto.Permissions;

public class PermissionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public List<PermissionDto> Children { get; set; } = new();
}
