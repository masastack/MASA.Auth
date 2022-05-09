namespace Masa.Auth.Contracts.Admin.Sso;

public class CustomLoginThirdPartyIdpDto
{
    public Guid Id { get; set; }

    public int Sort { get; set; }

    public CustomLoginThirdPartyIdpDto()
    {
    }

    public CustomLoginThirdPartyIdpDto(Guid id, int sort)
    {
        Id = id;
        Sort = sort;
    }
}

