namespace MASA.Auth.Sdk.ApiUri;

internal class Routing
{
    #region ServiceBaseAdress

    public const string AuthServiceBaseAdress = "http://localhost:8080/";

    #endregion

    #region UrlRule

    const string DEFAULT_SERVICE_BASE = "/api/{0}/{1}";

    const string DEFAULT_SERVICE = "/api/{0}";

    const string DEFAULT_SERVICE_LIST = "/api/{0}/items";

    #endregion

    #region Platform

    const string PLATFORM = "platform";

    public static readonly string PlatformList = string.Format(DEFAULT_SERVICE_LIST, PLATFORM);

    public static readonly string PlatformById = string.Format(DEFAULT_SERVICE_BASE, PLATFORM, "{id}");

    //public static readonly string PlatformDetail = string.Format(DEFAULT_SERVICE_BASE, $"{PLATFORM}Detail", "{id}");

    public static readonly string OperatePlatform = string.Format(DEFAULT_SERVICE, PLATFORM);

    //public static readonly string PlatformSelect = string.Format(DEFAULT_SERVICE_BASE, PLATFORM, "select");

    #endregion
}
