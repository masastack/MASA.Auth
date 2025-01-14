namespace Masa.Stack.Components.Models;

public partial class FavoriteNav
{
    public string CategoryCode { get; set; }

    public string AppCode { get; set; }

    public Nav Nav { get; set; }

    public FavoriteNav()
    {
    }

    public FavoriteNav(string categoryCode, string appCode, Nav nav)
    {
        CategoryCode = categoryCode;
        AppCode = appCode;
        Nav = nav;
    }
}

public partial class FavoriteNav
{
    public string Id => $"{CategoryCode}-{AppCode}-{Nav.Code}";
}