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

    /// <summary>
    /// 评估动态角色条件：先计算首个条件作为初值，后续条件用其 LogicalOperator 与累计结果合并。
    /// 无条件时返回 true。
    /// </summary>
    public async Task<bool> EvaluateConditionsAsync(User user, DynamicRole dynamicRole)
    {
        var conditions = dynamicRole.Conditions.OrderBy(c => c.Order).ToList();
        if (conditions.Count == 0) return true;

        // 计算第一个条件作为初始结果
        var first = conditions[0];
        var firstExtractor = _extractorFactory.GetExtractor(first.DataType);
        var firstValue = await firstExtractor.ExtractValueAsync(user, first.FieldName);
        var result = first.EvaluateCondition(firstValue);

        // 从第二个条件开始按其 LogicalOperator 与累计结果合并
        for (var i = 1; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            var extractor = _extractorFactory.GetExtractor(condition.DataType);
            var value = await extractor.ExtractValueAsync(user, condition.FieldName);
            var isMet = condition.EvaluateCondition(value);

            result = condition.LogicalOperator == LogicalOperator.And
                ? result && isMet
                : result || isMet;
        }

        return result;
    }
}
