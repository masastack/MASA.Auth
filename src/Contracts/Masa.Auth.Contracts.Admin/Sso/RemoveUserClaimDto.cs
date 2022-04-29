namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveUserClaimDto
{
    public int Id { get; set; }

    public RemoveUserClaimDto(int id)
    {
        Id = id;
    }
}

