namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class ThirdPartyUserDetailResponse
{
    public Guid ThirdPartyUserId { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; }

    public UserDetailResponse User { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public static ThirdPartyUserDetailResponse Default = new(default, default, true, "", UserDetailResponse.Default, DateTime.Now, null, "","");

    public ThirdPartyUserDetailResponse(Guid thirdPartyUserId, Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, UserDetailResponse user, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        ThirdPartyUserId = thirdPartyUserId;
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        User = user;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}


