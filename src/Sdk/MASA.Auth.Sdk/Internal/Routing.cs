namespace MASA.Auth.Sdk.Internal;

internal class Routing
{
    #region Authorize

    public static readonly string AuthorizeList = string.Format(UrlRule.DEFAULT_SERVICE_LIST, UrlRule.AUTHORIZE_SERVICE);

    #endregion

    #region Permission

    public static readonly string PermissionList = string.Format(UrlRule.DEFAULT_SERVICE_LIST, UrlRule.PERMISSION_SERVICE);

    public static readonly string PermissionDetail = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.PERMISSION_SERVICE, "{id}");

    public static readonly string OperatePermission = string.Format(UrlRule.DEFAULT_SERVICE, UrlRule.PERMISSION_SERVICE);

    #endregion

    #region Role

    public static readonly string RoleList = string.Format(UrlRule.DEFAULT_SERVICE_LIST, UrlRule.ROLE_SERVICE);

    public static readonly string RoleListByIds = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.ROLE_SERVICE, "ids");

    public static readonly string RoleDetail = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.ROLE_SERVICE, "{id}");

    public static readonly string OperateRole = string.Format(UrlRule.DEFAULT_SERVICE, UrlRule.ROLE_SERVICE);

    public static readonly string RoleSelect = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.ROLE_SERVICE, "select");

    #endregion

    #region Object

    public static string ObjectList = string.Format(UrlRule.DEFAULT_SERVICE_LIST, UrlRule.OBJECT_SERVICE);

    public static string ObjectAll = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.OBJECT_SERVICE, "all");

    public static string ContainsObject = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.OBJECT_SERVICE, "contains");

    public static string OperateObject = string.Format(UrlRule.DEFAULT_SERVICE, UrlRule.OBJECT_SERVICE);

    public static string BatchDeleteObject = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.OBJECT_SERVICE, "batchDelete");

    #endregion

    #region User

    public static string UserList = string.Format(UrlRule.DEFAULT_SERVICE_LIST, UrlRule.USER_SERVICE);

    public static string UserDetail = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.USER_SERVICE, "{id}");

    /// <summary>
    /// User additions, deletions and modifications use the same url
    /// </summary>
    public static string OperateUser = string.Format(UrlRule.DEFAULT_SERVICE, UrlRule.USER_SERVICE);

    public static string UserRole = string.Format(UrlRule.DEFAULT_SERVICE_BASE, UrlRule.USER_SERVICE, "role");

    #endregion
}
