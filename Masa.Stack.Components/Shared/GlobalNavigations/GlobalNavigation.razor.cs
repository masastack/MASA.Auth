namespace Masa.Stack.Components;

public partial class GlobalNavigation : MasaComponentBase
{
    private const string MENU_URL_NAME = "url";

    [Parameter]
    public string ClientId { get; set; }

    [Parameter]
    public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = null!;

    [Parameter]
    public Func<string, Task>? OnFavoriteAdd { get; set; }

    [Parameter]
    public Func<string, Task>? OnFavoriteRemove { get; set; }

    [Inject]
    public GlobalConfig GlobalConfig { get; set; } = null!;

    private bool _visible;
    private List<(string name, string url)>? _recentVisits;
    private List<KeyValuePair<string, string>>? _recommendApps;
    private List<ExpansionMenu>? _favorites;
    private ExpansionMenu? _menu;
    private string? _search;

    private async Task GetMenuAndFavorites()
    {
        _menu = await GenerateMenuAsync();
        _favorites = _menu.GetMenusByStates(ExpansionMenuState.Favorite);
        StateHasChanged();
    }

    private async Task GetRecommendApps()
    {
        //TODO pm config
        var recommendAppIdentities = new List<string>()
        {
            MasaStackConfig.GetWebId(MasaStackProject.PM), MasaStackConfig.GetWebId(MasaStackProject.DCC),
            MasaStackConfig.GetWebId(MasaStackProject.Auth)
        };
        var environment = MultiEnvironmentUserContext.Environment;
        if (environment.IsNullOrEmpty())
        {
            Logger.LogWarning("Multi Environment is empty");
            environment = WebHostEnvironment.Environment;
        }

        var projects = await PmClient.ProjectService.GetProjectAppsAsync(environment);
        _recommendApps = projects.SelectMany(p => p.Apps).Where(a => recommendAppIdentities.Contains(a.Identity))
                                 .Select(a => new KeyValuePair<string, string>(a.Name, a.Url)).ToList();

        StateHasChanged();
    }

    private void VisibleChanged(bool visible)
    {
        if (visible)
        {
            SearchChanged(null);

            if (_menu == null)
            {
                _ = GetRecommendApps();
                _ = GetRecentVisits();
                _ = GetMenuAndFavorites();
            }
        }

        _visible = visible;
    }

    private void SearchChanged(string? search)
    {
        _search = search;
        _menu?.SetHiddenBySearch(search, TranslateProvider);
    }

    private void MenuItemClickAsync(ExpansionMenu menu)
    {
        var url = menu.GetData(MENU_URL_NAME);
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        NavigateTo(url);
    }

    private async Task MenuItemOperClickAsync(ExpansionMenu menu)
    {
        if (menu.State == ExpansionMenuState.Normal)
        {
            await FavoriteRemoveAsync(menu);
        }
        else if (menu.State == ExpansionMenuState.Favorite)
        {
            await FavoriteAddAsync(menu);
        }
    }

    private async Task<ExpansionMenu> GenerateMenuAsync()
    {
        var menu = ExpansionMenu.CreateRootMenu(ExpansionMenuSituation.Favorite);
        try
        {
            var apps = (await AuthClient.ProjectService.GetGlobalNavigations(ClientId)).SelectMany(p => p.Apps).ToList();
            var categories = apps.GroupBy(a => a.Tag).ToList();
            var favorites = await FetchFavorites();

            foreach (var category in categories)
            {
                var categoryMenu = new ExpansionMenu(category.Key, category.Key, ExpansionMenuType.Category, ExpansionMenuState.Normal, menu.MetaData,
                    parent: menu);
                foreach (var app in category.Where(a => a.Navs.Any()))
                {
                    var appMenu = new ExpansionMenu(app.Id.ToString(), app.Name, ExpansionMenuType.App, ExpansionMenuState.Normal, menu.MetaData,
                        parent: categoryMenu);
                    foreach (var nav in app.Navs)
                    {
                        appMenu.AddChild(ConvertForNav(nav, appMenu.Deep + 1, appMenu, favorites));
                    }

                    categoryMenu.AddChild(appMenu);
                }

                menu.AddChild(categoryMenu);
            }
        }
        catch
        {
        }

        return menu;
    }

    private ExpansionMenu ConvertForNav(NavModel navModel, int deep, ExpansionMenu parent, List<string> favorites)
    {
        var state = favorites.Any(favorite => favorite == navModel.Code) ? ExpansionMenuState.Favorite : ExpansionMenuState.Normal;
        var menu = new ExpansionMenu(navModel.Code, navModel.Name, ExpansionMenuType.Nav, state, parent.MetaData, parent: parent)
            .AddData(MENU_URL_NAME, navModel.Url);
        foreach (var childrenNav in navModel.Children)
        {
            menu.AddChild(ConvertForNav(childrenNav, deep++, menu, favorites));
        }

        menu.Disabled = menu.Children.Count > 0;
        return menu;
    }

    private async Task<List<string>> FetchFavorites()
    {
        return (await AuthClient.PermissionService.GetFavoriteMenuListAsync())
               .Select(m => m.Value.ToString()).ToList();
    }

    private async Task GetRecentVisits()
    {
        var visitedList = await AuthClient.UserService.GetVisitedListAsync();
        _recentVisits = visitedList.Select(v => new ValueTuple<string, string>(v.Name, v.Url)).ToList();

        StateHasChanged();
    }

    private void NavigateTo(string? url)
    {
        if (url is null)
        {
            return;
        }

        NavigationManager.NavigateTo(url, forceLoad: true);
    }

    private async Task FavoriteRemoveAsync(ExpansionMenu nav)
    {
        var favoriteNav = _favorites.FirstOrDefault(e => e.Id == nav.Id);
        if (favoriteNav == null)
        {
            return;
        }

        _favorites.Remove(favoriteNav);
        if (OnFavoriteRemove != null)
        {
            await OnFavoriteRemove.Invoke(favoriteNav.Id);
        }
    }

    private async Task FavoriteAddAsync(ExpansionMenu nav)
    {
        if (_favorites.Any(e => e.Id == nav.Id))
        {
            return;
        }

        _favorites.Add(nav);
        if (OnFavoriteAdd != null)
        {
            await OnFavoriteAdd.Invoke(nav.Id);
        }
    }
}
