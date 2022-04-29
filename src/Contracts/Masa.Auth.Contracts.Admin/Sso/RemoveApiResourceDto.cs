namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveApiResourceDto
{
    public int Id { get; set; }

    public RemoveApiResourceDto(int id)
    {
        Id = id;
    }
}

