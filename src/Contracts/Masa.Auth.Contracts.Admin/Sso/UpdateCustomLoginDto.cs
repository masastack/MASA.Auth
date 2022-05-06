namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateCustomLoginDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Title { get; private set; } = "";

    public bool Enabled { get; set; }

    public List<CustomLoginThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    public UpdateCustomLoginDto()
    {
    }

    public UpdateCustomLoginDto(int id, string name, string title, bool enabled, List<CustomLoginThirdPartyIdpDto> thirdPartyIdps, List<RegisterFieldDto> registerFields)
    {
        Id = id;
        Name = name;
        Title = title;
        Enabled = enabled;
        ThirdPartyIdps = thirdPartyIdps;
        RegisterFields = registerFields;
    }
}

