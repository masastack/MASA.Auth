namespace Masa.Stack.Components.Layouts;

public partial class Favorites
{
    [Inject]
    private CookieStorage? CookieStorage { get; set; }

    [Parameter, EditorRequired]
    public List<Nav>? FlattenedNavs { get; set; }

    private const string GlobalConfig_Favorite = "GlobalConfig_Favorite";

    private bool _menuValue;
    private string? _searchKey;

    private List<string> FavoriteNavCodes { get; set; } = new();

    private List<FavoriteNav> InternalNavs { get; set; } = new();

    private IEnumerable<FavoriteNav> SearchedMenus { get; set; } = new List<FavoriteNav>();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        FlattenedNavs ??= new List<Nav>();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ArgumentNullException.ThrowIfNull(CookieStorage);

            var cookieFavorite = await CookieStorage.GetAsync(GlobalConfig_Favorite);

            if (cookieFavorite is not null)
            {
                FavoriteNavCodes = cookieFavorite.Split("|").ToList();
                InternalNavs = FlattenedNavs!.Select(nav => new FavoriteNav(nav, FavoriteNavCodes.Contains(nav.Code))).ToList();
                StateHasChanged();
            }
        }
    }

    private async Task HandleOnSearchKeyDown(KeyboardEventArgs args)
    {
        if (args.Code == "Enter")
        {
            await Task.Delay(128);
            SearchNavs(_searchKey);
        }
    }

    private void SearchNavs(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            SearchedMenus = InternalNavs.OrderBy((item) => !item.Favorite).ToList();
            return;
        }

        SearchedMenus = InternalNavs.Where(item => T(item.Nav.Name).Contains(key, StringComparison.OrdinalIgnoreCase))
                                    .OrderBy(item => !item.Favorite)
                                    .ToList();
    }

    private void ToggleFavorite(string code)
    {
        var nav = InternalNavs.FirstOrDefault(item => item.Nav.Code == code);
        if (nav is null) return;

        nav.Favorite = !nav.Favorite;

        if (nav.Favorite)
        {
            FavoriteNavCodes.Add(code);
        }
        else
        {
            FavoriteNavCodes.Remove(code);
        }

        CookieStorage.SetAsync(GlobalConfig_Favorite, string.Join("|", FavoriteNavCodes));
    }

    private void MenuValueChanged(bool value)
    {
        _menuValue = value;

        if (value)
        {
            _searchKey = null;
            SearchNavs(null);
        }
    }

    private void NavigateTo(string url)
    {
        NavigationManager.NavigateTo(url);
        _menuValue = false;
    }

    private bool IsMenuActive(Nav menu)
    {
        return (menu.IsActive(NavigationManager.ToBaseRelativePath(NavigationManager.Uri)));
    }

    public class FavoriteNav
    {
        public Nav Nav { get; }

        public bool Favorite { get; set; }

        public FavoriteNav(Nav nav, bool favorite)
        {
            Nav = nav;
            Favorite = favorite;
        }
    }
}
