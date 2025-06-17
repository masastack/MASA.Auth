// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Services;

/// <summary>
/// Dynamic role field value extractor for handling dynamic role validation result extraction
/// </summary>
public class DynamicRoleFieldValueExtractor : IFieldValueExtractor
{
    private readonly IDynamicRoleRepository _dynamicRoleRepository;
    private readonly Func<User, DynamicRole, Task<bool>> _evaluateConditionsFunc;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dynamicRoleRepository">Dynamic role repository</param>
    /// <param name="evaluateConditionsFunc">Condition evaluation function</param>
    public DynamicRoleFieldValueExtractor(
        IDynamicRoleRepository dynamicRoleRepository,
        Func<User, DynamicRole, Task<bool>> evaluateConditionsFunc)
    {
        _dynamicRoleRepository = dynamicRoleRepository;
        _evaluateConditionsFunc = evaluateConditionsFunc;
    }

    public DynamicRoleDataType SupportedDataType => DynamicRoleDataType.DynamicRole;

    /// <summary>
    /// Extract dynamic role validation result from user object
    /// </summary>
    /// <param name="user">User object</param>
    /// <param name="fieldName">Field name (dynamic role ID)</param>
    /// <returns>Validation result ("true" or "false")</returns>
    public async Task<string?> ExtractValueAsync(User user, string fieldName)
    {
        if (Guid.TryParse(fieldName, out var dynamicRoleId))
        {
            var dynamicRole = await _dynamicRoleRepository.FindAsync(dynamicRoleId);
            if (dynamicRole == null) return "false";

            var result = await _evaluateConditionsFunc(user, dynamicRole);
            return result.ToString();
        }
        throw new UserFriendlyException($"Invalid dynamic role ID: {fieldName}");
    }
}