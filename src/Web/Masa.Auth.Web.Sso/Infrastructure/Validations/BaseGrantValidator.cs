// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

/// <summary>
/// Base class for Grant Validators, provides common operation log recording functionality
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
    /// Record token acquisition operation log (including client information)
    /// </summary>
    /// <param name="user">User information</param>
    /// <param name="description">Operation description</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="loggerName">Logger name (for error logging)</param>
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