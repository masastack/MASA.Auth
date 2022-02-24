using MASA.Auth.Service.Domain.SSO.Aggregates.Abstract;

namespace MASA.Auth.Service.Domain.SSO.Aggregates.ApiResources;

public class ApiResourceClaim : UserClaim
{
    public int ApiResourceId { get; set; }
    public ApiResource ApiResource { get; set; } = null!;
}

