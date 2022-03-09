namespace Masa.Auth.Service.Infrastructure.Const;

public class Routing
{
    const string DEFAULT_SERVICE_BASE = "/api/{0}/{1}";

    const string DEFAULT_SERVICE = "/api/{0}";

    const string DEFAULT_SERVICE_LIST = "/api/{0}/list";

    #region module
    const string DEPARTMENT = "department";
    const string STAFF = "staff";
    const string ROLE = "role";
    const string TEAM = "team";
    const string USER = "user";
    const string PERMISSION = "permission";
    #endregion

    #region #Department

    public static readonly string DepartmentList = string.Format(DEFAULT_SERVICE_LIST, DEPARTMENT);

    public static readonly string Department = string.Format(DEFAULT_SERVICE, DEPARTMENT);

    #endregion

    #region Staff

    public static readonly string Staff = string.Format(DEFAULT_SERVICE, STAFF);

    public static readonly string StaffList = string.Format(DEFAULT_SERVICE_LIST, STAFF);

    public static readonly string StaffPagination = string.Format(DEFAULT_SERVICE_BASE, STAFF, "pagination");

    #endregion

    #region Permission

    public static readonly string Permission = string.Format(DEFAULT_SERVICE, PERMISSION);

    public static readonly string PermissionList = string.Format(DEFAULT_SERVICE_LIST, PERMISSION);

    public static readonly string PermissionDetail = string.Format(DEFAULT_SERVICE_BASE, PERMISSION, "{id}");

    #endregion
}
