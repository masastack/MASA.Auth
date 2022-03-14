namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientScope : Entity<int>
{
    public string Scope { get; set; } = "";

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
