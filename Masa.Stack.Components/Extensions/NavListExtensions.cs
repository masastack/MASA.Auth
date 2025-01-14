namespace Masa.Stack.Components.Extensions;

internal static class NavListExtensions
{
    public static string GetDefaultRoute(this List<Nav> navs, string defaultRoute = "403")
    {
        var firstMenu = navs.FirstOrDefault();
        if (firstMenu != null)
        {
            if (string.IsNullOrEmpty(firstMenu.Url))
            {
                return GetDefaultRoute(firstMenu.Children);
            }
            return firstMenu.Url;
        }
        return defaultRoute;
    }
}
