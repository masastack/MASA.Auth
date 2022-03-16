namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceScope : Entity<int>
{
    public string Scope { get; } = "";

    public int ApiResourceId { get; }

    public ApiResource ApiResource { get; } = null!;
}

