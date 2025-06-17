// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Services;

/// <summary>
/// Field value extractor factory for managing and retrieving different types of field value extractors
/// </summary>
public class FieldValueExtractorFactory
{
    private readonly IEnumerable<IFieldValueExtractor> _extractors;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="extractors">All registered field value extractors</param>
    public FieldValueExtractorFactory(IEnumerable<IFieldValueExtractor> extractors)
    {
        _extractors = extractors;
    }

    /// <summary>
    /// Get the corresponding field value extractor based on data type
    /// </summary>
    /// <param name="dataType">Data type</param>
    /// <returns>Field value extractor</returns>
    /// <exception cref="NotSupportedException">Unsupported data type</exception>
    public IFieldValueExtractor GetExtractor(DynamicRoleDataType dataType)
    {
        var extractor = _extractors.FirstOrDefault(e => e.SupportedDataType == dataType);
        if (extractor == null)
        {
            throw new NotSupportedException($"Unsupported data type: {dataType}");
        }

        return extractor;
    }

    /// <summary>
    /// Check if the specified data type is supported
    /// </summary>
    /// <param name="dataType">Data type</param>
    /// <returns>Whether it is supported</returns>
    public bool IsSupported(DynamicRoleDataType dataType)
    {
        return _extractors.Any(e => e.SupportedDataType == dataType);
    }
}