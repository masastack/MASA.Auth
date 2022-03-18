namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceSecret : Secret
{
    public int ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;
}

