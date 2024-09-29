// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class OperationLogRepository : Repository<AuthDbContext, OperationLog, Guid>, IOperationLogRepository
{
    readonly IUserContext _userContext;
    readonly ILogger<OperationLogRepository> _logger;
    readonly OperaterProvider _operaterProvider;

    public OperationLogRepository(
        AuthDbContext context,
        ILogger<OperationLogRepository> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        OperaterProvider operaterProvider) : base(context, unitOfWork)
    {
        _userContext = userContext;
        _logger = logger;
        _operaterProvider = operaterProvider;
    }

    public async Task AddDefaultAsync(OperationTypes operationType, string operationDescription, Guid? @operator = null)
    {
        try
        {
            @operator ??= _userContext.GetUserId<Guid>();

            if (@operator is not null && @operator != Guid.Empty)
            {
                var operatorName = (await _operaterProvider.GetUserAsync(@operator.Value))?.RealDisplayName ?? "";
                await AddAsync(new OperationLog(
                    @operator.Value, operatorName, operationType, default, operationDescription
                ));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Add operation log error");
        }
    }
}
