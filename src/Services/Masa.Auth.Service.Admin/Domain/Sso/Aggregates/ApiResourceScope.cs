namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ApiResourceScope : Entity<int>
{
    public string Scope { get; set; } = "";

    public int ApiResourceId { get; set; }

    public ApiResource ApiResource { get; set; } = null!;
}

