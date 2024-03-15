// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class ImpersonatedNavigation : AdminCompontentBase
{
    private const string MENU_URL_NAME = "url";

    [Parameter]
    public Guid UserId { get; set; }

    [Parameter]
    public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = null!;

    [Parameter]
    public Func<string, Task>? OnFavoriteAdd { get; set; }

    [Parameter]
    public Func<string, Task>? OnFavoriteRemove { get; set; }

    bool _visible;
    private List<(string name, string url)>? _recentVisits;
    private List<KeyValuePair<string, string>>? _recommendApps;
    private List<ExpansionMenu> _favorites = new();
    ExpansionMenu? _menu;
    string impersonationToken = string.Empty;

    private async Task GetMenuAndFavorites()
    {
        _menu = await GenerateMenuAsync();
        _favorites = _menu.GetMenusByStates(ExpansionMenuState.Favorite) ?? new();
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
            environment = WebHostEnvironment.EnvironmentName;
        }

        var projects = await PmClient.ProjectService.GetProjectAppsAsync(environment);
        _recommendApps = projects.SelectMany(p => p.Apps).Where(a => recommendAppIdentities.Contains(a.Identity))
                                 .Select(a => new KeyValuePair<string, string>(a.Name, a.Url)).ToList();

        StateHasChanged();
    }

    private async Task GetImpersonationToken()
    {
        var input = new ImpersonateInputModel
        {
            UserId = UserId
        };

        var impersonate = await AuthClient.UserService.ImpersonateAsync(input);
        if (impersonate != null)
        {
            impersonationToken = impersonate.ImpersonationToken;
        }
    }

    private void VisibleChanged(bool visible)
    {
        if (visible && _menu == null)
        {
            _ = GetRecommendApps();
            _ = GetRecentVisits();
            _ = GetMenuAndFavorites();
        }

        if (visible && string.IsNullOrEmpty(impersonationToken))
        {
            _ = GetImpersonationToken();
        }

        _visible = visible;
    }

    private void SearchChanged(string? search)
    {
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
            var apps = (await AuthClient.ProjectService.GetGlobalNavigations()).SelectMany(p => p.Apps).ToList();
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

        if (url.IndexOf("http") > -1)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Query = $"impersonationToken={impersonationToken}";
            url = uriBuilder.Uri.ToString();
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

    private async Task OnOutsideClick()
    {
        var a = 1;
    }
}
