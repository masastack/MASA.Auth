namespace Masa.Auth.Service.Domain.SSO.Aggregates.Abstract
{
    public abstract class UserClaim
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
    }
}
