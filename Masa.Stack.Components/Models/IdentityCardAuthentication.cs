namespace Masa.Stack.Components.Models;

public class IdentityCardAuthentication
{
    public string? UserId { get; set; }

    public string? Name { get; set; }

    public string? Number { get; set; }

    public DateTime? ValidityPeriodStartedAt { get; set; }

    public DateTime? ValidityPeriodEndedAt { get; set; }

    public string? FrontImageUrl { get; set; }

    public string? BackImageUrl { get; set; }
}
