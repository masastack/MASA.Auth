// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class OperationLogRepository : Repository<AuthDbContext, OperationLog, Guid>, IOperationLogRepository
{
    readonly IUserContext _userContext;
    readonly ILogger<OperationLogRepository> _logger;

    public OperationLogRepository(
        AuthDbContext context,
        ILogger<OperationLogRepository> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext) : base(context, unitOfWork)
    {
        _userContext = userContext;
        _logger = logger;
    }

    public async Task AddDefaultAsync(OperationTypes operationType, string operationDescription, Guid? @operator = null)
    {
        try
        {
            @operator ??= _userContext.GetUserId<Guid>();
            var operatorName = await Context.Set<User>()
                                            .Where(user => user.Id == @operator).Select(user => user.Name)
                                            .FirstAsync();
            await AddAsync(new OperationLog(
                @operator.Value, operatorName, operationType, default, operationDescription
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Add operation log error");
        }
    }
}
