namespace Masa.Auth.ApiGateways.Caller;

public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId");
        if (userClaim != null)
        {
            request.Headers.Add("user-id", userClaim.Value);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
