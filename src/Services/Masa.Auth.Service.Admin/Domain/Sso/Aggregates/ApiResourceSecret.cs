using Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceSecret : Secret
{
    public int ApiResourceId { get; }

    public ApiResource ApiResource { get; } = null!;
}

