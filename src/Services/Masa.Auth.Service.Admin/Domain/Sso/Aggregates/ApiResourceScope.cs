namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceScope : Entity<int>
{
    public string Scope { get; private set; } = "";

    public int ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;
}

