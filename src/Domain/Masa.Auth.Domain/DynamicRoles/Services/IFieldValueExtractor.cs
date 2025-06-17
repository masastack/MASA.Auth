// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Services;

/// <summary>
/// Field value extractor interface for extracting specific field values from user objects
/// </summary>
public interface IFieldValueExtractor
{
    /// <summary>
    /// Supported data type
    /// </summary>
    DynamicRoleDataType SupportedDataType { get; }

    /// <summary>
    /// Asynchronously extract the value of the specified field from the user object
    /// </summary>
    /// <param name="user">User object</param>
    /// <param name="fieldName">Field name</param>
    /// <returns>Field value</returns>
    Task<string?> ExtractValueAsync(User user, string fieldName);
}