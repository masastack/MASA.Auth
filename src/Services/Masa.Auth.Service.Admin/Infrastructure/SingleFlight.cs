// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Collections.Concurrent;

namespace Masa.Auth.Service.Admin.Infrastructure;

public class Call
{
    public ManualResetEventSlim ResetEventSlim { get; set; } = new ManualResetEventSlim(true);

    public Exception? Exception { get; set; }

    public object? Value { get; set; }

    public bool Forgotten { get; set; }
}

public class CallGroup : ISingletonDependency
{
    readonly IDictionary<string, Call> _callDic;

    readonly ReaderWriterLockSlim _lockSlim;

    public CallGroup()
    {
        _callDic = new ConcurrentDictionary<string, Call>();
        _lockSlim = new ReaderWriterLockSlim();
    }

    public T Do<T>(string key, Func<T> callFunc)
    {
        _lockSlim.EnterWriteLock();
        if (_callDic.ContainsKey(key))
        {
            var call = _callDic[key];
            _lockSlim.ExitWriteLock();

            // wait on first call to finish
            call.ResetEventSlim.Wait();
            return ResolveCallResult<T>(call);
        }
        var newCall = new Call();
        _callDic.Add(key, newCall);
        _lockSlim.ExitWriteLock();
        try
        {
            newCall.Value = callFunc.Invoke();
        }
        catch (Exception ex)
        {
            newCall.Exception = ex;
        }
        // stop all waiting 
        newCall.ResetEventSlim.Set();

        return ResolveCallResult<T>(newCall);
    }

    public void Forget(string key)
    {
        _lockSlim.EnterWriteLock();
        if (_callDic.ContainsKey(key))
        {
            _callDic[key].Forgotten = true;
            _callDic.Remove(key);
        }
        _lockSlim.ExitWriteLock();
    }

    T ResolveCallResult<T>(Call call)
    {
        if (call.Exception != null)
        {
            throw call.Exception;
        }
        if (call.Value == null)
        {
            throw new Exception("callFunc return null");
        }
        return (T)call.Value;
    }
}
