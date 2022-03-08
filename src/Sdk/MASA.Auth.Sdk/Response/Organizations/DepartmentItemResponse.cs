namespace MASA.Auth.Sdk.Response.Organizations;

public class DepartmentItemResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public List<DepartmentItemResponse> Children { get; set; } = new();
}

