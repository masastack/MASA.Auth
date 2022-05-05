namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateCustomLoginDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public bool Enabled { get; set; }

    public List<CustomLoginThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    public UpdateCustomLoginDto()
    {
    }

    public UpdateCustomLoginDto(int id, string name, bool enabled, List<CustomLoginThirdPartyIdpDto> thirdPartyIdps, List<RegisterFieldDto> registerFields)
    {
        Id = id;
        Name = name;
        Enabled = enabled;
        ThirdPartyIdps = thirdPartyIdps;
        RegisterFields = registerFields;
    }
}

