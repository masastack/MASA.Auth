// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

/// <summary>
/// Grant Validator基类，提供通用的操作日志记录功能
/// </summary>
public abstract class BaseGrantValidator
{
    protected readonly IAuthClient _authClient;
    protected readonly ILogger _logger;

    protected BaseGrantValidator(IAuthClient authClient, ILogger logger)
    {
        _authClient = authClient;
        _logger = logger;
    }

    /// <summary>
    /// 记录Token获取操作日志（包含客户端信息）
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <param name="description">操作描述</param>
    /// <param name="clientId">客户端ID</param>
    /// <param name="loggerName">日志记录器名称（用于错误日志）</param>
    protected async Task RecordTokenOperationLogAsync(UserModel user, string description, string? clientId, string loggerName = "")
    {
        try
        {
            var operationLogModel = new AddOperationLogModel(
                user.Id,
                user.DisplayName,
                OperationTypes.Login,
                DateTime.UtcNow,
                description,
                clientId);

            await _authClient.OperationLogService.AddLogAsync(operationLogModel);
        }
        catch (Exception ex)
        {
            // 记录日志失败不应该影响Token获取流程，只记录错误
            _logger.LogError(ex, "Failed to record token operation log for user {UserId} in {LoggerName}", user.Id, loggerName);
        }
    }
} 