namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDetailDto : UserDto
{ 
    public AddressValueDto Address { get; set; }

    public List<string> ThirdPartyIdpAvatars { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public DateTime? ModificationTime { get; set; }

    public static UserDetailDto Default => new UserDetailDto(Guid.Empty, "", "", "", "", "", "", default, "", "", DateTime.Now, new(), new(), "","",default);

    public UserDetailDto(Guid userId, string name, string displayName, string avatar, string iDCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime, AddressValueDto address, List<string> thirdPartyIdpAvatars, string creator, string modifier, DateTime? modificationTime) : base(userId, name, displayName, avatar, iDCard, account, companyName, enabled, phoneNumber, email, creationTime)
    {
        Address = address;
        ThirdPartyIdpAvatars = thirdPartyIdpAvatars;
        Creator = creator;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }
}

