namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientGrantType : Entity<int>
{
    public string GrantType { get; } = "";

    public int ClientId { get; }

    public Client Client { get; } = null!;
}

