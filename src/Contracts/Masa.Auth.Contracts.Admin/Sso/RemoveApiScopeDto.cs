namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveApiScopeDto
{
    public int Id { get; set; }

    public RemoveApiScopeDto(int id)
    {
        Id = id;
    }
}

