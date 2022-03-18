namespace Masa.Auth.Contracts.Admin.Infrastructure.Models;

public class AvatarValueDto
{
    public string Url { get; set; } = "";

    public string Name { get; set; } = "";

    public string Color { get; set; } = "";

    public AvatarValueDto()
    {

    }

    public AvatarValueDto(string url, string name, string color)
    {
        Url = url;
        Name = name;
        Color = color;
    }
}
