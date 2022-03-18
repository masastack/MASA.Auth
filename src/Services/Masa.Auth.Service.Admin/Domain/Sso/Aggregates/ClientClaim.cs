namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientClaim : Entity<int>
{
    public string Type { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
