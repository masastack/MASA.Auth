// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services;

public abstract class ServiceBase
{
    protected ICallerProvider CallerProvider { get; init; }

    protected abstract string BaseUrl { get; set; }

    protected ServiceBase(ICallerProvider callerProvider)
    {
        CallerProvider = callerProvider;
    }

    protected async Task<TResponse> GetAsync<TResponse>(string methodName, Dictionary<string, string>? paramters = null)
    {
        return await CallerProvider.GetAsync<TResponse>(BuildAdress(methodName), paramters ?? new()) ?? throw new UserFriendlyException("The service is abnormal, please contact the administrator!");
    }

    protected async Task<TResponse> GetAsync<TRequest, TResponse>(string methodName, TRequest data) where TRequest : class
    {
        return await CallerProvider.GetAsync<TRequest, TResponse>(BuildAdress(methodName), data) ?? throw new UserFriendlyException("The service is abnormal, please contact the administrator!");
    }

    protected async Task PutAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PutAsync(BuildAdress(methodName), data);
    }

    protected async Task PostAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PostAsync(BuildAdress(methodName), data);
    }

    protected async Task<TResponse> PostAsync<TResponse>(string methodName, HttpContent content)
    {
        var response = await CallerProvider.PostAsync(BuildAdress(methodName), content);
        var json = await response.Content.ReadAsStringAsync();
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true
        };
        return JsonSerializer.Deserialize<TResponse>(json, serializeOptions) ?? throw new UserFriendlyException("Internal error, please contact administrator");
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string methodName, TRequest data)
    {
        return await CallerProvider.PostAsync<TRequest, TResponse>(BuildAdress(methodName), data) ?? throw new UserFriendlyException("The service is abnormal, please contact the administrator!");
    }

    protected async Task DeleteAsync<TRequest>(string methodName, TRequest? data = default)
    {
        var response = await CallerProvider.DeleteAsync(BuildAdress(methodName), data);
    }

    protected async Task DeleteAsync(string methodName)
    {
        var response = await CallerProvider.DeleteAsync(BuildAdress(methodName), null);
    }

    protected async Task SendAsync<TRequest>(string methodName, TRequest? data = default)
    {
        if (methodName.StartsWith("Add")) await PostAsync(methodName, data);
        else if (methodName.StartsWith("Update")) await PutAsync(methodName, data);
        else if (methodName.StartsWith("Remove")) await DeleteAsync(methodName, data);
    }

    protected async Task<TResponse> SendAsync<TRequest, TResponse>(string methodName, TRequest data) where TRequest : class
    {
        return await CallerProvider.GetAsync<TRequest, TResponse>(BuildAdress(methodName), data) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    string BuildAdress(string methodName)
    {
        return Path.Combine(BaseUrl, methodName.Replace("Async", ""));
    }
}

