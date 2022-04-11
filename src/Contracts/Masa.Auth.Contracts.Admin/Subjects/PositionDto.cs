namespace Masa.Auth.Contracts.Admin.Subjects;

public class PositionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public PositionDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}


