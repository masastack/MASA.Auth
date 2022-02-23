namespace MASA.Auth.Service.Domain.SSO.Aggregates.ApiResources;

public class ApiResourceScope
{
    public int Id { get; set; }
    public string Scope { get; set; } = "";

    public int ApiResourceId { get; set; }
    public ApiResource ApiResource { get; set; } = null!;
}

