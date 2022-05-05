namespace Masa.Auth.Contracts.Admin.Sso;

public class CustomLoginDetailDto : CustomLoginDto
{
    public List<CustomLoginThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    public CustomLoginDetailDto() { }

    public CustomLoginDetailDto(int id, string name, ClientDto client, bool enabled, DateTime creationTime, DateTime? modificationTime, string creator, string modifier, List<CustomLoginThirdPartyIdpDto> thirdPartyIdps, List<RegisterFieldDto> registerFields) : base(id, name, client, enabled, creationTime, modificationTime, creator, modifier)
    {
        ThirdPartyIdps = thirdPartyIdps;
        RegisterFields = registerFields;
    }

    public static implicit operator UpdateCustomLoginDto(CustomLoginDetailDto customLogin)
    {
        return new UpdateCustomLoginDto(customLogin.Id, customLogin.Name, customLogin.Enabled, customLogin.ThirdPartyIdps, customLogin.RegisterFields);
    }
}

