namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientIdPRestriction : Entity<int>
{
    public string Provider { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
