// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class OperationLogRepository : Repository<AuthDbContext, OperationLog, Guid>, IOperationLogRepository
{
    IUserContext _userContext;

    public OperationLogRepository(
        AuthDbContext context,
        IUnitOfWork unitOfWork,
        IUserContext userContext) : base(context, unitOfWork)
    {
        _userContext = userContext;
    }

    public async Task AddDefaultAsync(OperationTypes operationType, string operationDescription)
    {
        var @operator = _userContext.GetUserId<Guid>();
        var operatorName = await Context.Set<User>().Where(user => user.Id == @operator).Select(user => user.Name).FirstAsync();
        await AddAsync(new OperationLog(
            @operator, operatorName, operationType, DateTime.Now, operationDescription
        ));
    }
}
