namespace Masa.Auth.Contracts.Admin.Projects;

public class PermissionNavDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<PermissionNavDto> Children { get; set; } = new();
}
