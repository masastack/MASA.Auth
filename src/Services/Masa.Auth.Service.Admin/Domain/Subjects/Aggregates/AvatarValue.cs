namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class AvatarValue : ValueObject
{
    public string Url { get; set; }

    public string Name { get; set; } 

    public string Color { get; set; }

    public AvatarValue(string url, string name, string color)
    {
        Url = url;
        Name = name;
        Color = color;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Url;
        yield return Name;
        yield return Color;
    }
}
