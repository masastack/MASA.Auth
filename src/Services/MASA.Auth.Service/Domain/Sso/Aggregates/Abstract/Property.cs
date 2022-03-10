namespace Masa.Auth.Service.Domain.Sso.Aggregates.Abstract;

public abstract class Property : Entity<Guid>
{
    public string Key { get; set; } = "";

    public string Value { get; set; } = "";
}
