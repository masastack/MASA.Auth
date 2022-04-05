namespace Masa.Auth.Contracts.Admin.Organizations;

public class UpdatePositionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public UpdatePositionDto()
    {
        Name = "";
    }

    public UpdatePositionDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
