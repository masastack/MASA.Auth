namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveIdentityResourceDto
{
    public int Id { get; set; }


    public RemoveIdentityResourceDto(int id)
    {
        Id = id;
    }
}

