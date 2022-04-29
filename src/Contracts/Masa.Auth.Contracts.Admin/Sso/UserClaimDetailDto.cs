namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimDetailDto : UserClaimDto
{
    public UserClaimDetailDto() { }

    public UserClaimDetailDto(int id, string name, string description, UserClaimType userClaimType) : base(id, name, description, userClaimType)
    {
    }

    public static implicit operator UpdateUserClaimDto(UserClaimDetailDto userClaim)
    {
        return new UpdateUserClaimDto(userClaim.Id, userClaim.Name, userClaim.Description, userClaim.UserClaimType);
    }
}

