// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyUser : FullAggregateRoot<Guid, Guid>
{
    private User? _user;
    private readonly User? _createUser;
    private readonly User? _modifyUser;
    private IdentityProvider _identityProvider = default!;

    private Guid _thirdPartyIdpId;
    private Guid _userId;
    private string _thridPartyIdentity = "";
    private string _extendedData = "";

    public Guid ThirdPartyIdpId
    {
        get => _thirdPartyIdpId;
        set => _thirdPartyIdpId = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(ThirdPartyIdpId));
    }

    public Guid UserId
    {
        get => _userId;
        private set => _userId = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(UserId));
    }

    public bool Enabled { get; private set; } = true;

    public string ThridPartyIdentity
    {
        get => _thridPartyIdentity;
        private set => _thridPartyIdentity = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(ThridPartyIdentity));
    }

    public string ExtendedData
    {
        get => _extendedData;
        private set => _extendedData = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(ExtendedData));
    }

    public User User => _user ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_FOUND);

    public User? CreateUser => _createUser;

    public User? ModifyUser => _modifyUser;

    public IdentityProvider IdentityProvider => _identityProvider;

    public ThirdPartyUser(Guid thirdPartyIdpId, string thridPartyIdentity, string extendedData)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        ThridPartyIdentity = thridPartyIdentity;
        ExtendedData = extendedData;
    }

    public ThirdPartyUser(Guid thirdPartyIdpId, Guid userId, string thridPartyIdentity, string extendedData)
            : this(thirdPartyIdpId, thridPartyIdentity, extendedData)
    {
        UserId = userId;
    }

    public void Update(string extendedData)
    {
        ExtendedData = extendedData;
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    public void Update(string thridPartyIdentity, string extendedData)
    {
        ThridPartyIdentity = thridPartyIdentity;
        ExtendedData = extendedData;
    }

    public static implicit operator ThirdPartyUserDetailDto(ThirdPartyUser tpu)
    {
        return new ThirdPartyUserDetailDto(tpu.Id, tpu.Enabled, tpu.IdentityProvider.Adapt<IdentityProviderDetailDto>(), tpu.User, tpu.CreationTime, tpu.ModificationTime, tpu.CreateUser?.Name ?? "", tpu.ModifyUser?.Name ?? "");
    }
}

