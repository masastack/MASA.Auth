namespace Masa.Auth.Contracts.Admin.Subjects;

public class RemoveStaffDto
{
    public Guid Id { get; set; }

    public RemoveStaffDto(Guid id)
    {
        Id = id;
    }
}

