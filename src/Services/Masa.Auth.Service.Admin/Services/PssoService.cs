// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Psso;

namespace Masa.Auth.Service.Admin.Services;

public class PssoService : ServiceBase
{
    IHttpContextAccessor _httpContextAccessor => GetRequiredService<IHttpContextAccessor>();
    IHttpClientFactory _httpClientFactory => GetRequiredService<IHttpClientFactory>();
    IMasaConfiguration _masaConfiguration => GetRequiredService<IMasaConfiguration>();

    public PssoService() : base("api/psso")
    {
        RouteOptions.DisableAutoMapRoute = false;
    }

    private string? GetUserId()
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        var userId = user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        return userId;
    }

    private string? GetUserType()
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        var userId = user?.FindFirst("http://Lonsid.org/identity/claims/userType")?.Value;
        return userId;
    }

    private async Task<AbpApiResponse<TResult>> CheckConvertResult<TResult>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errMsg = response.ReasonPhrase.IsNullOrEmpty() ? "Service call failed" : response.ReasonPhrase;
            throw new UserFriendlyException(errMsg);
        }

        var result = await response.Content.ReadAsStringAsync();

        if ((int)response.StatusCode == 299)
        {
            throw new UserFriendlyException(result);
        }
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        return JsonSerializer.Deserialize<AbpApiResponse<TResult>>(result, options)!;
    }

    private HttpClient CreateClient()
    {
        var soaOptions = _masaConfiguration.ConfigurationApi.GetPublic().GetSection(SoaOptions.Key).Get<SoaOptions>();
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(soaOptions.ServerUrl);
        return client;
    }

    [AllowAnonymous]
    public async Task<GetUserInfoOutput> GetUserInfoAsync()
    {
        var client = CreateClient();
        var userId = GetUserId();
        var userType = GetUserType();
        var response = await client.GetAsync($"/api/pssom/lonsidUser/getUserInfo/{userId}?userType={userType}");
        return (await CheckConvertResult<GetUserInfoOutput>(response)).Result!;
    }

    public async Task<GetPermissionsByLonsidUserIdOutput> GetPermissionsAsync()
    {
        var client = CreateClient();
        var userId = GetUserId();
        var response = await client.GetAsync($"/api/pssom/lonsidUser/getPermissions/{userId}");
        return (await CheckConvertResult<GetPermissionsByLonsidUserIdOutput>(response)).Result!;
    }

    public async Task<List<FeatureTreeDto>> GetFeatureTreesByUserAsync(string moduleName)
    {
        var client = CreateClient();
        var userId = GetUserId();
        var response = await client.GetAsync($"/api/pssom/role/getFeatureTreesByUser/{userId}?moduleName={moduleName}");
        return (await CheckConvertResult<List<FeatureTreeDto>>(response)).Result!;
    }

    public async Task<List<GetUserFavoriteFeatureOutput>> GetUserFavoriteFeatureAsync(string moduleName)
    {
        var client = CreateClient();
        var userId = GetUserId();
        var response = await client.GetAsync($"/api/pssom/role/getUserFavoriteFeature/{userId}?moduleName={moduleName}");
        return (await CheckConvertResult<List<GetUserFavoriteFeatureOutput>>(response)).Result!;
    }
}