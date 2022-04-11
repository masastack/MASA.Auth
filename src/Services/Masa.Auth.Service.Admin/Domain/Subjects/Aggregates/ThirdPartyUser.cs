namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyUser : AuditAggregateRoot<Guid, Guid>
{
    private User? _user;
    private ThirdPartyIdp? _thirdPartyIdp;

    public User User => _user ?? LazyLoader?.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public ThirdPartyIdp ThirdPartyIdp => _thirdPartyIdp ?? LazyLoader?.Load(this, ref _thirdPartyIdp) ?? throw new UserFriendlyException("Failed to get thirdPartyIdp data");

    public Guid ThirdPartyIdpId { get; private set; }

    public Guid UserId { get; private set; }

    public bool Enabled { get; private set; }

    public string ThridPartyIdentity { get; private set; }

    private ILazyLoader? LazyLoader { get; set; }

    public ThirdPartyUser(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
        ThridPartyIdentity = "";
    }

    public ThirdPartyUser(Guid thirdPartyIdpId, Guid userId, bool enabled, string thridPartyIdentity)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        UserId = userId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
    }
}

