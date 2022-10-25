// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimDetailDto : UserClaimDto
{
    public UserClaimDetailDto() { }

    public UserClaimDetailDto(Guid id, string name, string description, UserClaimType userClaimType) : base(id, name, description, userClaimType)
    {
    }

    public static implicit operator UpdateUserClaimDto(UserClaimDetailDto userClaim)
    {
        return new UpdateUserClaimDto(userClaim.Id, userClaim.Name, userClaim.Description, userClaim.UserClaimType);
    }
}

