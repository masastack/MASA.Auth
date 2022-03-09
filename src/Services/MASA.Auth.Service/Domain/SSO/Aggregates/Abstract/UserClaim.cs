namespace Masa.Auth.Service.Domain.Sso.Aggregates.Abstract
{
    public abstract class UserClaim : Entity<int>
    {
        public string Type { get; set; } = "";
    }
}
