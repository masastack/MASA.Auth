namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract
{
    public abstract class UserClaim : Entity<int>
    {
        public string Type { get; } = "";
    }
}
