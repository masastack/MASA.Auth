namespace Masa.Auth.ApiGateways.Caller.Infrastructure.Models;

public class AvatarValue
{
    public string Url { get; set; } = "";

    public string Name { get; set; } = "";

    public string Color { get; set; } = "";

    public AvatarValue()
    {

    }

    public AvatarValue(string url, string name, string color)
    {
        Url = url;
        Name = name;
        Color = color;
    }
}
