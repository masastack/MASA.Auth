namespace MASA.Auth.Service.Domain.Aggregate;

public class Permission : Entity<int>
{
    public string AppId { get; set; } = "";

    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public string Description { get; set; } = "";

    public State State { get; set; }

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public PermissionType Type { get; set; }

    private List<PermissionItem> permissionItems = new();

    public IReadOnlyCollection<PermissionItem> PermissionItems => permissionItems;

}

