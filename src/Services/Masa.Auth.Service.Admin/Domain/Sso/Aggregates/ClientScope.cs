namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientScope : Entity<int>
{
    public string Scope { get; } = "";

    public int ClientId { get; }

    public Client Client { get; } = null!;
}
