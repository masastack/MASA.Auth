namespace Masa.Auth.Contracts.Admin.Subjects;

public class RemoveUserDto
{
    Guid Id { get; set; }

    public RemoveUserDto(Guid id)
    {
        Id = id;
    }
}

