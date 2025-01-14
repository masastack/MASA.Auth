
namespace Masa.Stack.Components.Extensions.OpenIdConnect;

public class LogoutSessionManager
{
    private readonly ILogger<LogoutSessionManager> _logger;
    //todo expired
    readonly ConcurrentDictionary<string, BcLogoutSession> _sessions = new ConcurrentDictionary<string, BcLogoutSession>();

    public LogoutSessionManager(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<LogoutSessionManager>();
    }

    public void Add(string sub, string sid)
    {
        _logger.LogDebug($"Backchannel add a logout to the session: sub: {sub}, sid: {sid}");
        var logoutSession = new BcLogoutSession(sub, sid);
        _sessions[sub + sid] = logoutSession;
    }

    public bool IsLoggedOut(string sub, string sid)
    {
        _logger.LogDebug($"Backchannel IsLoggedOutAsync: sub: {sub}, sid: {sid}");
        var key = sub + sid;
        var matches = false;
        _sessions.TryGetValue(key, out var logoutSession);
        if (logoutSession != null)
        {
            matches = logoutSession.IsMatch(sub, sid);
            _logger.LogDebug($"Backchannel Logout session exists T/F {matches} : {sub}, sid: {sid}");
        }
        return matches;
    }
}

class BcLogoutSession
{
    public string Sub { get; set; }

    public string Sid { get; set; }

    public BcLogoutSession(string sub, string sid)
    {
        Sub = sub;
        Sid = sid;
    }

    public bool IsMatch(string sub, string sid)
    {
        return Sid == sid && Sub == sub ||
               Sid == sid && Sub == null ||
               Sid == null && Sub == sub;
    }
}
