// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

/// <summary>
/// In-process segmented gate that serializes "check-then-act" flows on a string key
/// (third-party identity, account, ...).
/// <list type="bullet">
/// <item>SegmentLock: keyed by any string identity, e.g. two concurrent logins/upserts of the
/// same external account cannot both pass the existence check and create duplicate rows.</item>
/// <item>BatchSyncLock: single-flight for the bulk LDAP sync job.</item>
/// </list>
/// NOTE: process-local only. For multi-instance deployments the final guarantee is the filtered
/// unique indexes (User.Account/Email/... and ThirdPartyUser(ThirdPartyIdpId, ThridPartyIdentity))
/// combined with the DbUpdateException fallbacks in the domain services.
/// </summary>
public static class SegmentedAsyncGate
{
    private const int SegmentCount = 32;

    private static readonly SemaphoreSlim[] SegmentLocks = Enumerable
        .Range(0, SegmentCount)
        .Select(_ => new SemaphoreSlim(1, 1))
        .ToArray();

    private static readonly SemaphoreSlim BatchSyncLock = new(1, 1);

    /// <summary>
    /// Get the lock segment for one key. The hash is only used to pick a segment,
    /// so a per-process randomized string hash is fine.
    /// </summary>
    public static SemaphoreSlim GetSegmentLock(string key)
    {
        var index = (uint)StringComparer.Ordinal.GetHashCode(key ?? string.Empty) % SegmentCount;
        return SegmentLocks[index];
    }

    /// <summary>
    /// Run <paramref name="func"/> exclusively under the segment lock of <paramref name="key"/>.
    /// Wraps the WaitAsync/try/finally/Release boilerplate of the check-then-act call sites.
    /// </summary>
    public static async Task<T> RunExclusiveAsync<T>(string key, Func<Task<T>> func)
    {
        var gate = GetSegmentLock(key);
        await gate.WaitAsync();
        try
        {
            return await func();
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>
    /// Try to enter the batch sync and run <paramref name="func"/>; returns false when another
    /// sync is still running (the caller should skip instead of queueing).
    /// </summary>
    public static async Task<bool> TryRunBatchSyncAsync(Func<Task> func)
    {
        if (!BatchSyncLock.Wait(0))
            return false;
        try
        {
            await func();
            return true;
        }
        finally
        {
            BatchSyncLock.Release();
        }
    }
}
