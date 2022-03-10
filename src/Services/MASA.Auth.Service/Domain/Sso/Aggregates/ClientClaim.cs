namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientClaim
{
    public string Type { get; set; } = String.Empty;

    public string Value { get; set; } = String.Empty;

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
