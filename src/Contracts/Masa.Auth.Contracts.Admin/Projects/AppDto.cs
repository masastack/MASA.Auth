namespace Masa.Auth.Contracts.Admin.Projects;

public class AppDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Identity { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    public int ProjectId { get; set; }
}
