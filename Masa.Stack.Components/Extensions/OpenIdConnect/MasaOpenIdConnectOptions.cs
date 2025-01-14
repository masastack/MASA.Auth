namespace Masa.Stack.Components.Extensions.OpenIdConnect;

public class MasaOpenIdConnectOptions
{
    public string Authority { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public List<string> Scopes { get; set; } = new List<string>();
}