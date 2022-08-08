// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using HttpMethods = Masa.BuildingBlocks.BasicAbility.Scheduler.Enum.HttpMethods;

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class SyncUserAutoCompleteExtensions
{
    public static async Task AddSyncUserAutoCompleteJobAsync(this IServiceCollection services)
    {
        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        var authUrl = scope.ServiceProvider
                          .GetRequiredService<IMasaConfiguration>()
                          .ConfigurationApi
                          .GetDefault()
                          .GetValue<string>("AppSettings:AuthClient:Url");
        var schedulerClient = scope.ServiceProvider.GetRequiredService<ISchedulerClient>();
        await schedulerClient.SchedulerJobService.AddAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = "masa-auth",
            Name = "SyncUserAutoCompleteJob",
            IsAlertException = true,
            JobType = JobTypes.Http,
            CronExpression = "0 0 0 * * ? *",
            Description = "SyncUserAutoCompleteJob",
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.ExecuteImmediately,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Cover,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.RunFailedStrategy,
            RunTimeoutSecond = 12 * 60 * 60,
            FailedRetryInterval = 10,
            FailedRetryCount = 3,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.POST,
                RequestUrl = authUrl,
                HttpBody = JsonSerializer.Serialize(new SyncUserAutoCompleteDto { OnceExecuteCount = 1000 }),
            }
        });
    }
}
