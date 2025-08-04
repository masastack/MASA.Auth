// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Services;

public class RoleFieldValueExtractor : IFieldValueExtractor
{
    public DynamicRoleDataType SupportedDataType => DynamicRoleDataType.Role;

    public Task<string?> ExtractValueAsync(User user, string fieldName)
    {
        if (Guid.TryParse(fieldName, out var roleId))
        {
            var result = user.Roles.Any(x => x.RoleId == roleId);
            return Task.FromResult<string?>(result.ToString());
        }
        throw new UserFriendlyException($"Invalid role ID: {fieldName}");
    }
}
