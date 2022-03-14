namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientSecret : Secret
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
