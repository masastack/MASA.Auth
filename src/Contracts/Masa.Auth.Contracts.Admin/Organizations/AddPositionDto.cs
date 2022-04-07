namespace Masa.Auth.Contracts.Admin.Organizations;

public class AddPositionDto
{
    public string Name { get; set; }

    public AddPositionDto()
    {
        Name = "";
    }

    public AddPositionDto(string name)
    {
        Name = name;
    }
}
