namespace Masa.Auth.Contracts.Admin.Subjects;

public class RemoveUserDto
{
    public Guid Id { get; set; }

    public RemoveUserDto(Guid id)
    {
        Id = id;
    }
}

