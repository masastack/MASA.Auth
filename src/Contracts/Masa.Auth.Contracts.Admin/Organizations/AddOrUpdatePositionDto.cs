namespace Masa.Auth.Contracts.Admin.Organizations;

public class AddOrUpdatePositionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public AddOrUpdatePositionDto()
    {
    }

    public AddOrUpdatePositionDto(Guid id, string name)
    {
        Name = name;
    }
}
