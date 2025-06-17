// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Services;

/// <summary>
/// User claim field value extractor for extracting user claim field values
/// </summary>
public class UserClaimFieldValueExtractor : IFieldValueExtractor
{
    public DynamicRoleDataType SupportedDataType => DynamicRoleDataType.UserClaim;

    /// <summary>
    /// Extract user claim field value from user object
    /// </summary>
    /// <param name="user">User object</param>
    /// <param name="fieldName">Field name</param>
    /// <returns>Field value</returns>
    public Task<string?> ExtractValueAsync(User user, string fieldName)
    {
        var value = user.UserClaims.FirstOrDefault(x => x.Name == fieldName)?.Value;
        return Task.FromResult(value);
    }
}