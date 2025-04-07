// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Jobs;

public class SyncUserJob : BackgroundJobBase<SyncUserArgs>
{
    readonly IServiceProvider _serviceProvider;

    public SyncUserJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(SyncUserArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(args.Environment);
        var userDomainService = scope.ServiceProvider.GetRequiredService<UserDomainService>();
        await userDomainService.SyncUserAsync(args.UserId);
    }
}