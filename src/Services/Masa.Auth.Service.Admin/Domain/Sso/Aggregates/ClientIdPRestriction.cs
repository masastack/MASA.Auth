namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientIdPRestriction : Entity<int>
{
    public string Provider { get; } = string.Empty;

    public int ClientId { get; }

    public Client Client { get; } = null!;
}
