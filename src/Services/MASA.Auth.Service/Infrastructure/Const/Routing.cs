namespace Masa.Auth.Service.Infrastructure.Const;

public class Routing
{
    const string DEFAULT_SERVICE_BASE = "/api/{0}/{1}";

    const string DEFAULT_SERVICE = "/api/{0}";

    const string DEFAULT_SERVICE_LIST = "/api/{0}/list";

    #region module
    const string DEPARTMENT = "department";
    const string STAFF = "staff";
    const string PERMISSION = "permission";
    #endregion

    #region Department

    public static readonly string DepartmentList = string.Format(DEFAULT_SERVICE_LIST, DEPARTMENT);

    public static readonly string Department = string.Format(DEFAULT_SERVICE, DEPARTMENT);

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

    #region Staff

    public static readonly string Staff = string.Format(DEFAULT_SERVICE, STAFF);

    public static readonly string StaffList = string.Format(DEFAULT_SERVICE_LIST, STAFF);

    public static readonly string StaffPagination = string.Format(DEFAULT_SERVICE_BASE, STAFF, "pagination");

    public static readonly string OperateStaff = string.Format(DEFAULT_SERVICE_BASE, STAFF);

    #endregion

    #region Permission

    public static readonly string Permission = string.Format(DEFAULT_SERVICE, PERMISSION);

    public static readonly string PermissionList = string.Format(DEFAULT_SERVICE_LIST, PERMISSION);

    public static readonly string PermissionDetail = string.Format(DEFAULT_SERVICE_BASE, PERMISSION, "{id}");

    #endregion
}
