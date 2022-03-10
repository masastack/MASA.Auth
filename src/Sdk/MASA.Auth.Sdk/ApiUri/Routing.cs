namespace Masa.Auth.Sdk.ApiUri;

internal class Routing
{
    #region ServiceBaseAdress

    public const string AUTH_SERVICE_BASE_ADRESS = "http://localhost:8080/";

    #endregion

    #region UrlRule

    const string DEFAULT_SERVICE_BASE = "/api/{0}/{1}";

    const string DEFAULT_SERVICE = "/api/{0}";

    const string DEFAULT_SERVICE_LIST = "/api/{0}/items";

    #endregion

    #region User

    const string USER = "user";

    public static readonly string UserList = string.Format(DEFAULT_SERVICE_LIST, USER);

    public static readonly string UserDetail = string.Format(DEFAULT_SERVICE_BASE, USER, "{id}");

    public static readonly string OperateUser = string.Format(DEFAULT_SERVICE_BASE, USER);

    #endregion

    #region Platform

    const string PLATFORM = "platform";

    public static readonly string PlatformList = string.Format(DEFAULT_SERVICE_LIST, PLATFORM);

    public static readonly string PlatformDetail = string.Format(DEFAULT_SERVICE_BASE, PLATFORM, "{id}");

    public static readonly string OperatePlatform = string.Format(DEFAULT_SERVICE, PLATFORM);

    #endregion

    #region PlatformUser

    const string PLATFORM_USER = "platformUser";

    public static readonly string PLATFORMUserList = string.Format(DEFAULT_SERVICE_LIST, PLATFORM_USER);

    public static readonly string PLATFORMUserDetail = string.Format(DEFAULT_SERVICE_BASE, PLATFORM_USER, "{id}");

    public static readonly string OperatePlatformUser = string.Format(DEFAULT_SERVICE_BASE, PLATFORM_USER);

    #endregion
}
