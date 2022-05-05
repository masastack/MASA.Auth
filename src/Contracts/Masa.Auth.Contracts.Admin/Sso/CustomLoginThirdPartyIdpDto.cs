namespace Masa.Auth.Contracts.Admin.Sso;

public class CustomLoginThirdPartyIdpDto
{
    public Guid ThirdPartyIdpId { get; private set; }

    public int Sort { get; private set; }

    public CustomLoginThirdPartyIdpDto()
    {
    }

    public CustomLoginThirdPartyIdpDto(Guid thirdPartyIdpId, int sort)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Sort = sort;
    }
}

