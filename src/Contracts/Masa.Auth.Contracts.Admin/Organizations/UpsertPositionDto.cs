namespace Masa.Auth.Contracts.Admin.Organizations;

public class UpsertPositionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public UpsertPositionDto()
    {
    }

    public UpsertPositionDto(Guid id, string name)
    {
        Name = name;
    }
}
