namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientClaim : Entity<int>
{
    public string Type { get; } = string.Empty;

    public string Value { get; } = string.Empty;

    public int ClientId { get; }

    public Client Client { get; } = null!;
}
