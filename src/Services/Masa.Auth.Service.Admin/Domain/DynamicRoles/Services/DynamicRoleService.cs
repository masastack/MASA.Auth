// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.DynamicRoles.Services;

public class DynamicRoleService : DomainService
{
    private readonly ILogger<DynamicRoleService> _logger;
    private readonly IDynamicRoleRepository _dynamicRoleRepository;
    private readonly FieldValueExtractorFactory _extractorFactory;

    public DynamicRoleService(
        ILogger<DynamicRoleService> logger,
        IDynamicRoleRepository dynamicRoleRepository,
        FieldValueExtractorFactory extractorFactory)
    {
        _logger = logger;
        _dynamicRoleRepository = dynamicRoleRepository;
        _extractorFactory = extractorFactory;
    }

    public async Task<bool> EvaluateConditionsAsync(User user, Guid roleId)
    {
        var dynamicRole = await _dynamicRoleRepository.FindAsync(roleId);
        if (dynamicRole == null) return false;
        return await EvaluateConditionsAsync(user, dynamicRole);
    }

    public async Task<bool> EvaluateConditionsAsync(User user, DynamicRole dynamicRole)
    {
        if (!dynamicRole.Conditions.Any()) return true; // No conditions means always true.

        foreach (var condition in dynamicRole.Conditions.OrderBy(c => c.Order))
        {
            var extractor = _extractorFactory.GetExtractor(condition.DataType);
            var value = await extractor.ExtractValueAsync(user, condition.FieldName);

            var result = condition.EvaluateCondition(value);
            if (result && condition.LogicalOperator == LogicalOperator.Or)
                return result;
            else if (!result && condition.LogicalOperator == LogicalOperator.And)
                return false;
        }
        return true;
    }
}
