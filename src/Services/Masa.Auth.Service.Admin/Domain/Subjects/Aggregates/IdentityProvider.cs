// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class IdentityProvider : FullAggregateRoot<Guid, Guid>
{
    List<ThirdPartyUser> _thirdPartyUsers = new();
    string _name = "";
    string _displayName = "";
    string _icon = "";
    ThirdPartyIdpTypes _thirdPartyIdpType;

    public string Name
    {
        get => _name;
        protected set => _name = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(Name));
    }

    public string DisplayName
    {
        get => _displayName;
        protected set => _displayName = value ?? "";
    }

    public string Icon
    {
        get => _icon;
        protected set => _icon = value ?? "";
    }

    public bool Enabled { get; protected set; } = true;

    public ThirdPartyIdpTypes ThirdPartyIdpType
    {
        get => _thirdPartyIdpType;
        protected set => _thirdPartyIdpType = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(ThirdPartyIdpType));
    }

    public IReadOnlyCollection<ThirdPartyUser> ThirdPartyUsers => _thirdPartyUsers;

    public void ClearThirdPartyUser()
    {
        _thirdPartyUsers.Clear();
    }
}
