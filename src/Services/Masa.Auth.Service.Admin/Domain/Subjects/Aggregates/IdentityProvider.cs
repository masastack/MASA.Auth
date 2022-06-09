// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class IdentityProvider : FullAggregateRoot<Guid, Guid>
{
    List<ThirdPartyUser> _thirdPartyUsers = new();

    public string Name { get; protected set; } = null!;

    public string DisplayName { get; protected set; } = null!;

    public string Icon { get; protected set; } = null!;

    public bool Enabled { get; protected set; } = true;

    public IdentificationTypes IdentificationType { get; protected set; }

    public IReadOnlyCollection<ThirdPartyUser> ThirdPartyUsers => _thirdPartyUsers;

    public static implicit operator IdentityProviderDetailDto(IdentityProvider tpIdp)
    {
        return new IdentityProviderDetailDto(tpIdp.Id, tpIdp.Name, tpIdp.DisplayName, tpIdp.Icon, tpIdp.Enabled);
    }
}
