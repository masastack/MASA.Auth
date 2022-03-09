using Masa.Auth.Service.Infrastructure.Enums;

namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class ThirdPartyUser : Entity<Guid>
{
    public Guid ThirdPartyPlatformId { get; private set; }

    public Guid UserId { get; private set; }

    public bool Enabled { get; private set; }

    public ThirdPartyUser(Guid thirdPartyPlatformId, Guid userId, bool enabled)
    {
        ThirdPartyPlatformId = thirdPartyPlatformId;
        UserId = userId;
        Enabled = enabled;
    }
}

