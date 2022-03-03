namespace MASA.Auth.Sdk.Response.Organizations;

public class DepartmentItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public List<DepartmentItem> Children { get; set; } = new();
}

