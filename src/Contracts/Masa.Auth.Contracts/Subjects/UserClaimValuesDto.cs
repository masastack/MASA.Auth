namespace Masa.Auth.Contracts.Admin;

public class UserClaimValuesDto
{
    public Guid UserId { get; set; }

    public Dictionary<string, string> ClaimValues { get; set; } = new();
}
