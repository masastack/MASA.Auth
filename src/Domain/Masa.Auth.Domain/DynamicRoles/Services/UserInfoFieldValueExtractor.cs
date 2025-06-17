// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Services;

/// <summary>
/// User info field value extractor for extracting user basic information field values
/// </summary>
public class UserInfoFieldValueExtractor : IFieldValueExtractor
{
    public DynamicRoleDataType SupportedDataType => DynamicRoleDataType.UserInfo;

    /// <summary>
    /// Extract user info field value from user object
    /// </summary>
    /// <param name="user">User object</param>
    /// <param name="fieldName">Field name</param>
    /// <returns>Field value</returns>
    public Task<string?> ExtractValueAsync(User user, string fieldName)
    {
        var property = typeof(User).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        var value = property?.GetValue(user)?.ToString();
        return Task.FromResult(value);
    }
}