namespace Masa.Auth.Service.Domain.SSO.Aggregates;

public class ClientGrantType
{
    public int Id { get; set; }
    public string GrantType { get; set; } = "";
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}

