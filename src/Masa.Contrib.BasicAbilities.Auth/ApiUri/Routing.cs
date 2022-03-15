namespace Masa.Auth.ApiGateways.Caller.ApiUri;

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

    #region ThirdPartyPlatform

    const string THIRD_PARTY_PLATFORM = "thirdPartyPlatform";

    public static readonly string PlatformList = string.Format(DEFAULT_SERVICE_LIST, THIRD_PARTY_PLATFORM);

    public static readonly string PlatformDetail = string.Format(DEFAULT_SERVICE_BASE, THIRD_PARTY_PLATFORM, "{id}");

    public static readonly string PlatformSelect = string.Format(DEFAULT_SERVICE_BASE, THIRD_PARTY_PLATFORM, "select");

    public static readonly string OperatePlatform = string.Format(DEFAULT_SERVICE, THIRD_PARTY_PLATFORM);

    #endregion

    #region User

    const string USER = "user";

    public static readonly string UserList = string.Format(DEFAULT_SERVICE_LIST, USER);

    public static readonly string UserDetail = string.Format(DEFAULT_SERVICE_BASE, USER, "{id}");

    public static readonly string OperateUser = string.Format(DEFAULT_SERVICE_BASE, USER);

    #endregion

    #region ThirdPartyUser

    const string THIRD_PARTY_USER = "thirdPartyUser";

    public static readonly string ThirdPartyUserList = string.Format(DEFAULT_SERVICE_LIST, THIRD_PARTY_USER);

    public static readonly string ThirdPartyUserDetail = string.Format(DEFAULT_SERVICE_BASE, THIRD_PARTY_USER, "{id}");

    public static readonly string OperateThirdPartyUser = string.Format(DEFAULT_SERVICE_BASE, THIRD_PARTY_USER);

    #endregion

    #region Staff

    const string STAFF = "staff";

    public static readonly string StaffList = string.Format(DEFAULT_SERVICE_LIST, STAFF);

    public static readonly string StaffDetail = string.Format(DEFAULT_SERVICE_BASE, STAFF, "{id}");

    public static readonly string OperateStaff = string.Format(DEFAULT_SERVICE_BASE, STAFF);

    #endregion

    #region Role

    const string ROLE = "role";

    public static readonly string RoleList = string.Format(DEFAULT_SERVICE_LIST, ROLE);

    public static readonly string RoleDetail = string.Format(DEFAULT_SERVICE_BASE, ROLE, "{id}");

    public static readonly string RoleSelect = string.Format(DEFAULT_SERVICE_BASE, ROLE, "select");

    public static readonly string OperateRole = string.Format(DEFAULT_SERVICE_BASE, ROLE);

    #endregion

    #region Team

    const string TEAM = "team";

    public static readonly string TeamList = string.Format(DEFAULT_SERVICE_LIST, TEAM);

    public static readonly string TeamDetail = string.Format(DEFAULT_SERVICE_BASE, TEAM, "{id}");

    public static readonly string TeamSelect = string.Format(DEFAULT_SERVICE_BASE, TEAM, "select");

    public static readonly string OperateTeam = string.Format(DEFAULT_SERVICE_BASE, TEAM);

    #endregion
}
