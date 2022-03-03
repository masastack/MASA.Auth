using MASA.Auth.Sdk.Response;
using MASA.Auth.Sdk.Response.Subjects;

namespace MASA.Auth.Sdk.Callers.Subjects;

internal class PlatformCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    internal PlatformCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(PlatformCaller);
        BaseAddress = Routing.AuthServiceBaseAdress;
    }

    public async Task<ApiResultResponse<PaginatedItemResponse<PlatformItem>>> GetListAsync(int pageIndex = 1, int pageSize = 20, string? search = null)
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
            var response = await CallerProvider.GetAsync<PaginatedItemResponse<PlatformItem>>(Routing.PlatformList, queryArguments);
            return response!;
        });
    }
}
