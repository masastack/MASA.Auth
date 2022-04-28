namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateUserClaimDto : AddUserClaimDto
{
    public int Id { get; set; }

    public UpdateUserClaimDto()
    {
    }

    public UpdateUserClaimDto(int id,string name, string description, UserClaimType userClaimType) : base(name, description, userClaimType)
    {
        Id = id;
    }
}

