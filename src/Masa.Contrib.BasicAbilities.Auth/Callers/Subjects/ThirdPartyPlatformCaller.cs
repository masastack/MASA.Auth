using Masa.Contrib.BasicAbilities.Auth.ApiUri;
using Masa.Contrib.BasicAbilities.Auth.Callers;
using Masa.Contrib.BasicAbilities.Auth.Response;
using Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

namespace Masa.Contrib.BasicAbilities.Auth.Callers.Subjects;

internal class ThirdPartyPlatformCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    internal ThirdPartyPlatformCaller(IServiceProvider serviceProvider, Options options) : base(serviceProvider)
    {
        Name = nameof(ThirdPartyPlatformCaller);
        BaseAddress = options.AuthServiceBaseAdress;
    }

    public async Task<ApiResultResponse<PaginatedItemResponse<ThirdPartyPlatformItemResponse>>> GetListAsync(int pageIndex = 1, int pageSize = 20, string? search = null)
    {
        var queryArguments = new Dictionary<string, string>()
        {
            { "pageIndex", pageIndex.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search ?? "" },
        };

        return await ResultAsync(async () =>
        {
            //var url = QueryHelpers.AddQueryString(Routing.UserList, queryArguments);
            var response = await CallerProvider.GetAsync<PaginatedItemResponse<ThirdPartyPlatformItemResponse>>(Routing.PlatformList, queryArguments);
            return response!;
        });
    }
}
