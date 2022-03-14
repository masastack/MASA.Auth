namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ApiResourceProperty : Property
{
    public int ApiResourceId { get; set; }

    public ApiResource ApiResource { get; set; } = null!;
}

