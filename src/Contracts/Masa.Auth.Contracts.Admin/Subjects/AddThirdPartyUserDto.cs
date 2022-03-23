namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddThirdPartyUserDto
{
    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; }

    public AddUserDto User { get; set; }

    public AddThirdPartyUserDto(Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, AddUserDto user)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        User = user;
    }

    public static implicit operator AddThirdPartyUserDto(ThirdPartyUserDetailDto thridPartyUser)
    {
        return new AddThirdPartyUserDto(thridPartyUser.ThirdPartyIdpId, thridPartyUser.Enabled, thridPartyUser.ThridPartyIdentity, thridPartyUser.User);
    }
}
