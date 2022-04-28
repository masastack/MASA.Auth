namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimSelectDto : UserClaimDto
{
    public UserClaimSelectDto(int id, string name, string description, UserClaimType userClaimType) : base(id, name, description, userClaimType)
    {
    }
}

