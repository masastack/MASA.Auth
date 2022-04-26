namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyUser : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    private User? _user;
    private User? _createUser;
    private User? _modifyUser;

    private ThirdPartyIdp? _thirdPartyIdp;

    public bool IsDeleted { get; private set; }

    public User User => _user ?? LazyLoader?.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public User? CreateUser => _createUser ?? LazyLoader?.Load(this, ref _createUser);

    public User? ModifyUser => _modifyUser ?? LazyLoader?.Load(this, ref _modifyUser);

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

    public static implicit operator ThirdPartyUserDetailDto(ThirdPartyUser tpu)
    {
        return new ThirdPartyUserDetailDto(tpu.Id, tpu.Enabled, tpu.ThirdPartyIdp, tpu.User, tpu.CreationTime, tpu.ModificationTime, tpu.CreateUser?.Name ?? "", tpu.ModifyUser?.Name ?? "");
    }
}

