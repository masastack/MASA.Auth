namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientCorsOrigin : Entity<int>
{
    public string Origin { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
