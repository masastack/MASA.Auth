namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveCustomLoginDto
{
    public int Id { get; set; }

    public RemoveCustomLoginDto(int id)
    {
        Id = id;
    }
}

