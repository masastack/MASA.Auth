namespace Masa.Auth.Service.Admin.Domain.Projects.Aggregates;

public class AppNavigationTag : AggregateRoot<Guid>
{
    public string AppIdentity { get; private set; }

    public string Tag { get; private set; }

    public AppNavigationTag(string appIdentity, string tag)
    {
        AppIdentity = appIdentity;
        Tag = tag;
    }

    public void UpdateTag(string tag)
    {
        Tag = tag;
    }
}
