namespace Masa.Auth.Service.Domain.SSO.Aggregates;

public class ApiResourceSecret : Secret
{
    public int ApiResourceId { get; set; }
    public ApiResource ApiResource { get; set; } = null!;
}

