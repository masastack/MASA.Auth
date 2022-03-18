namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientGrantType : Entity<int>
{
    public string GrantType { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}

