// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using HttpMethods = Masa.BuildingBlocks.StackSdks.Scheduler.Enum.HttpMethods;

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class SchedulerJobServiceCollectionExtensions
{
    public static async Task AddSchedulerJobAsync(this IServiceCollection services)
    {
        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        await serviceProvider.SafeExcuteAsync(AddSyncUserAutoCompleteJobAsync);
        await serviceProvider.SafeExcuteAsync(AddSyncUserRedisJobAsync);
        await serviceProvider.SafeExcuteAsync(AddSyncOidcRedisJobAsync);
        await serviceProvider.SafeExcuteAsync(AddSyncStaffRedisJobAsync);
        await serviceProvider.SafeExcuteAsync(AddSyncPermissionRedisJobAsync);
        await serviceProvider.SafeExcuteAsync(AddSyncTeamRedisJobAsync);
    }

    static async Task CreateAndRunAsync(this IServiceProvider serviceProvider, AddSchedulerJobRequest addSchedulerJobRequest)
    {
        var schedulerClient = serviceProvider.GetRequiredService<ISchedulerClient>();
        var jobId = await schedulerClient.SchedulerJobService.AddAsync(addSchedulerJobRequest);
        await schedulerClient.SchedulerJobService.StartAsync(new SchedulerJobRequestBase
        {
            JobId = jobId
        });
    }

    public static async Task AddSyncUserAutoCompleteJobAsync(this IServiceProvider serviceProvider)
    {
        var authUrl = serviceProvider.GetRequiredService<IMasaStackConfig>().GetAuthServiceDomain();
        await serviceProvider.CreateAndRunAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.AUTH,
            JobIdentity = "masa-auth-sync-userAutoComplete-job",
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
                RequestUrl = Path.Combine(authUrl, "api/user/SyncUserAutoComplete/"),
                HttpBody = JsonSerializer.Serialize(new SyncUserAutoCompleteDto { OnceExecuteCount = 1000 }),
            }
        });
    }

    public static async Task AddSyncUserRedisJobAsync(this IServiceProvider serviceProvider)
    {
        var authUrl = serviceProvider.GetRequiredService<IMasaStackConfig>().GetAuthServiceDomain();
        await serviceProvider.CreateAndRunAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.AUTH,
            JobIdentity = "masa-auth-sync-syncUserRedis-job",
            Name = "SyncUserRedisJob",
            IsAlertException = true,
            JobType = JobTypes.Http,
            CronExpression = "0 0 0 * * ? *",
            Description = "SyncUserRedisJob",
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.ExecuteImmediately,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Cover,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.RunFailedStrategy,
            RunTimeoutSecond = 12 * 60 * 60,
            FailedRetryInterval = 10,
            FailedRetryCount = 3,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.POST,
                RequestUrl = Path.Combine(authUrl, "api/user/SyncRedis/"),
                HttpBody = JsonSerializer.Serialize(new SyncUserRedisDto { OnceExecuteCount = 1000 }),
            }
        });
    }

    public static async Task AddSyncOidcRedisJobAsync(this IServiceProvider serviceProvider)
    {
        var authUrl = serviceProvider.GetRequiredService<IMasaStackConfig>().GetAuthServiceDomain();
        await serviceProvider.CreateAndRunAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.AUTH,
            JobIdentity = "masa-auth-sync-syncOidcRedis-job",
            Name = "SyncOidcRedisJob",
            IsAlertException = true,
            JobType = JobTypes.Http,
            CronExpression = "0 0 0 * * ? *",
            Description = "SyncUserRedisJob",
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.ExecuteImmediately,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Cover,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.RunFailedStrategy,
            RunTimeoutSecond = 12 * 60 * 60,
            FailedRetryInterval = 10,
            FailedRetryCount = 3,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.POST,
                RequestUrl = Path.Combine(authUrl, "api/sso/client/SyncOidc/")
            }
        });
    }

    public static async Task AddSyncStaffRedisJobAsync(this IServiceProvider serviceProvider)
    {
        var authUrl = serviceProvider.GetRequiredService<IMasaStackConfig>().GetAuthServiceDomain();
        await serviceProvider.CreateAndRunAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.AUTH,
            JobIdentity = "masa-auth-sync-syncStaffRedis-job",
            Name = "SyncStaffRedisJob",
            IsAlertException = true,
            JobType = JobTypes.Http,
            CronExpression = "0 0 0 * * ? *",
            Description = "SyncStaffRedisJob",
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.ExecuteImmediately,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Cover,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.RunFailedStrategy,
            RunTimeoutSecond = 12 * 60 * 60,
            FailedRetryInterval = 10,
            FailedRetryCount = 3,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.POST,
                RequestUrl = Path.Combine(authUrl, "api/staff/SyncCache/")
            }
        });
    }

    public static async Task AddSyncPermissionRedisJobAsync(this IServiceProvider serviceProvider)
    {
        var authUrl = serviceProvider.GetRequiredService<IMasaStackConfig>().GetAuthServiceDomain();
        await serviceProvider.CreateAndRunAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.AUTH,
            JobIdentity = "masa-auth-sync-syncPermissionRedis-job",
            Name = "SyncPermissionRedisJob",
            IsAlertException = true,
            JobType = JobTypes.Http,
            CronExpression = "0 0 0 * * ? *",
            Description = "SyncPermissionRedisJob",
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.ExecuteImmediately,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Cover,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.RunFailedStrategy,
            RunTimeoutSecond = 12 * 60 * 60,
            FailedRetryInterval = 10,
            FailedRetryCount = 3,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.POST,
                RequestUrl = Path.Combine(authUrl, "api/permission/SyncRedis")
            }
        });
    }

    public static async Task AddSyncTeamRedisJobAsync(this IServiceProvider serviceProvider)
    {
        var authUrl = serviceProvider.GetRequiredService<IMasaStackConfig>().GetAuthServiceDomain();
        await serviceProvider.CreateAndRunAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.AUTH,
            JobIdentity = "masa-auth-sync-syncTeamRedis-job",
            Name = "SyncTeamRedisJob",
            IsAlertException = true,
            JobType = JobTypes.Http,
            CronExpression = "0 0 0 * * ? *",
            Description = "SyncTeamRedisJob",
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.ExecuteImmediately,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Cover,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.RunFailedStrategy,
            RunTimeoutSecond = 12 * 60 * 60,
            FailedRetryInterval = 10,
            FailedRetryCount = 3,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.POST,
                RequestUrl = Path.Combine(authUrl, "api/team/SyncRedis")
            }
        });
    }

    async static Task SafeExcuteAsync(this IServiceProvider serviceProvider, Func<IServiceProvider, Task> job, [CallerArgumentExpression("job")] string? jobName = null)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            await job(serviceProvider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"sync scheduler {jobName} error");
        }
    }
}
