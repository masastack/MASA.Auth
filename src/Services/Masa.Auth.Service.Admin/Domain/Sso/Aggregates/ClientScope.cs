namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientScope : Entity<int>
{
    public string Scope { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
