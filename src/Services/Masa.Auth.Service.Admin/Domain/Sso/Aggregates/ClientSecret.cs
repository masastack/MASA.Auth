using Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientSecret : Secret
{
    public int ClientId { get; }

    public Client Client { get; } = null!;
}
