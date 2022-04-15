namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class AvatarValue : ValueObject
{
    public string Url { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Color { get; private set; } = string.Empty;

    public AvatarValue(string name, string color)
    {
        Name = name;
        Color = color;
    }

    public AvatarValue(string url)
    {
        Url = url;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Url;
        yield return Name;
        yield return Color;
    }
}
