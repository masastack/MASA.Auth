namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientGrantType : Entity<int>
{
    public string GrantType { get; set; } = "";

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}

