namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

public abstract class Property : Entity<Guid>
{
    public string Key { get; private set; } = "";

    public string Value { get; private set; } = "";
}
