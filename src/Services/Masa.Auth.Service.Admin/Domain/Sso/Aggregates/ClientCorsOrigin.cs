namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientCorsOrigin : Entity<int>
{
    public string Origin { get; } = string.Empty;

    public int ClientId { get; }

    public Client Client { get; } = null!;
}
