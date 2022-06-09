// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyUser : FullAggregateRoot<Guid, Guid>
{
    private User? _user;
    private User? _createUser;
    private User? _modifyUser;
    private IdentityProvider? _identityProvider;

    public User User => _user ?? LazyLoader?.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public User? CreateUser => _createUser;

    //todo It will be deleted later
    public new Guid? Creator { get; private set; }

    public User? ModifyUser => _modifyUser;

    public new Guid? Modifier { get; private set; }

    public IdentityProvider IdentityProvider => (_identityProvider ?? LazyLoader?.Load(this, ref _identityProvider)) ?? throw new UserFriendlyException("Failed to get IdentityProvider data");

    public Guid ThirdPartyIdpId { get; private set; }

    public Guid UserId { get; private set; }

    public bool Enabled { get; private set; }

    public string ThridPartyIdentity { get; private set; }

    private ILazyLoader? LazyLoader { get; set; }

    private ThirdPartyUser(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
        ThridPartyIdentity = string.Empty;
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
        return new ThirdPartyUserDetailDto(tpu.Id, tpu.Enabled, tpu.IdentityProvider, tpu.User, tpu.CreationTime, tpu.ModificationTime, tpu.CreateUser?.Name ?? "", tpu.ModifyUser?.Name ?? "");
    }
}

