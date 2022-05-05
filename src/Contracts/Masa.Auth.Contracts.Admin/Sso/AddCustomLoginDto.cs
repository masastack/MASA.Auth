namespace Masa.Auth.Contracts.Admin.Sso;

public class AddCustomLoginDto
{
    public string Name { get; set; } = "";

    public int ClientId { get; set; }

    public bool Enabled { get; set; }

    public List<CustomLoginThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    public AddCustomLoginDto()
    {

    }

    public AddCustomLoginDto(string name, int clientId, bool enabled, List<CustomLoginThirdPartyIdpDto> thirdPartyIdps, List<RegisterFieldDto> registerFields)
    {
        Name = name;
        ClientId = clientId;
        Enabled = enabled;
        ThirdPartyIdps = thirdPartyIdps;
        RegisterFields = registerFields;
    }
}

