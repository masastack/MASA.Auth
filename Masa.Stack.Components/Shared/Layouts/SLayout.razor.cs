namespace Masa.Stack.Components;

public partial class SLayout
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private I18n I18n { get; set; } = null!;

    [Inject]
    private MasaUser MasaUser { get; set; } = null!;

    [Inject]
    private IMultiEnvironmentMasaStackConfig MultiEnvironmentMasaStackConfig { get; set; } = null!;

    [Inject]
    private IMultiEnvironmentUserContext MultiEnvironmentUserContext { get; set; } = null!;

    [Inject]
    public JsInitVariables JsInitVariables { get; set; } = default!;

    [Inject]
    public I18nCache I18nCache { get; set; } = default!;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? TeamRouteFormat { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter, EditorRequired]
    public string? Logo { get; set; }

    [Parameter, EditorRequired]
    public string? MiniLogo { get; set; }

    [Inject]
    internal ProjectAppOptions ProjectApp { get; set; } = default!;

    [Parameter]
    public Func<bool>? OnSignOut { get; set; }

    [Parameter]
    public Func<Exception, Task>? OnErrorAsync { get; set; }

    [Parameter]
    public RenderFragment<Exception>? ErrorContent { get; set; }

    [Parameter]
    public EventCallback<Exception> OnErrorAfterHandle { get; set; }

    [Parameter]
    public List<string> WhiteUris { get; set; } = new List<string>();

    [Parameter]
    public bool IsShowEnvironmentSwitch { get; set; } = false;

    private Breadcrumbs? _breadcrumbsComp;
    private Action? _breadcrumbSetCallback;

    private Breadcrumbs? BreadcrumbsComp
    {
        get => _breadcrumbsComp;
        set
        {
            _breadcrumbsComp = value;
            if (value is null)
            {
                return;
            }

            _breadcrumbSetCallback?.Invoke();
        }
    }

    [Parameter]
    public string? AppId { get; set; }

    public string GetAppId() => MultiEnvironmentMasaStackConfig.SetEnvironment(MultiEnvironmentUserContext.Environment ?? "").GetWebId(ProjectApp.Project);

    List<Nav> NavItems = new();
    List<string> _preWhiteUris = new();

    List<Nav> FlattenedNavs { get; set; } = new();

    List<Nav> FlattenedAllNavs { get; set; } = new();

    List<string> _whiteUriList = new List<string>
    {
        "403", "404", "user-center",
        "notification-center"
    };

    string ClientId
    {
        get
        {
            return AppId.IsNullOrEmpty() ? GetAppId() : AppId;
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (WhiteUris.Any() && !WhiteUris.SequenceEqual(_preWhiteUris))
        {
            _preWhiteUris = WhiteUris;
            _whiteUriList.AddRange(WhiteUris);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            GlobalConfig.Initialization();

            await JsInitVariables.SetTimezoneOffset();
            List<MenuModel> menus = new();

            if (!CheckAuthenticated())
            {
                return;
            }

            try
            {
                var appId = AppId.IsNullOrEmpty() ? GetAppId() : AppId;
                menus = await AuthClient.PermissionService.GetMenusAsync(appId);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "AuthClient.PermissionService.GetMenusAsync");
            }

            NavItems = menus.Adapt<List<Nav>>();

#if DEBUG
            if (Debugger.IsAttached && !NavItems.Any())
            {
                NavItems = new List<Nav>()
                {
                    new Nav("dashboard", "Dashboard", "mdi-view-dashboard-outline", "/index"),
                    new Nav("user", "User", "mdi-account", "/users"),
                    new Nav("products", "Products", "mdi-rectangle", "/products/list", matchPattern: "/products/(list|details/)[^/]*"),
                    new Nav("counter", "Counter", "mdi-pencil", "/counter"),
                    new Nav("fetchdata", "Fetch data", "mdi-delete", "/fetchdata"),
                    new Nav("father", "Father", null, new List<Nav>
                    {
                        new Nav("children2", "ChildTwo", "father", new List<Nav>()
                        {
                            new Nav("children", "ChildOne", default, "/children2", "children2"),
                        }),
                        new Nav("dialog", "dialog", default, "/dialog", "father"),
                        new Nav("tab", "tab", default, "/tab", "father"),
                        new Nav("sselect", "sselect", default, "/sselect", "father"),
                        new Nav("deleteButton", "deleteButton", default, "/deleteButton", "father"),
                        new Nav("mini", "mini", default, "/mini-components", "father"),
                        new Nav("extend", "extend", default, "/extend", "father"),
                        new Nav("userAutoCompleteExample", "userAutoComplete", default, "/userAutoCompleteExample", "father"),
                        new Nav("defaultTextFieldExample", "defaultTextField", default, "/defaultTextFieldExample", "father"),
                        new Nav("defaultButtonExample", "defaultButton", default, "/defaultButtonExample", "father"),
                        new Nav("defaultDataTableExample", "defaultDataTable", default, "/defaultDataTableExample", "father"),
                        new Nav("paginationExample", "pagination", default, "/defaultPaginationExample", "father"),
                        new Nav("uploadImageExample", "uploadImage", default, "/uploadImageExample", "father"),
                        new Nav("comboxExample", "combox", default, "/comboxExample", "father"),
                        new Nav("paginationSelectExample", "paginationSelect", default, "/paginationSelectExample", "father"),
                        new Nav("dateRangePickerExample", "dateRangePicker", default, "/dateRangePickerExample", "father"),
                        new Nav("dateTimeRangePickerExample", "dateTimeRangePicker", default, "/dateTimeRangePickerExample", "father"),
                        new Nav("simpleModalExample", "simpleModal", default, "/simpleModalExample", "father"),
                        new Nav("simpleConfirmExample", "simpleConfirmExample", default, "/simpleConfirmExample", "father"),
                        new Nav("empty", "empty", default, "/empty", "father"),
                        new Nav("icons", "icons", default, "/icons", "father"),
                        new Nav("searchPanelExample", "searchPanelExample", default, "/searchPanelExample", "father"),
                    }),
                };
            }
#endif

            GlobalConfig.Menus = NavItems;

            FlattenedNavs = FlattenNavs(NavItems, true);
            FlattenedAllNavs = FlattenNavs(NavItems, false);

            //add home index content sould remove this code
            if (NavigationManager.Uri == NavigationManager.BaseUri)
            {
                NavigationManager.NavigateTo(NavItems.GetDefaultRoute());
                return;
            }

            var absolutePath = NavigationManager.GetAbsolutePath();
            if (absolutePath.Contains("dashboard") is false && !IsMenusUri(NavItems, absolutePath))
            {
                NavigationManager.NavigateTo("/403");
                return;
            }

            Logger.LogInformation("URL of navigation to : {Location}", NavigationManager.Uri);
            try
            {
                await AuthClient.UserService.VisitedAsync(AppId, absolutePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "AuthClient.UserService.VisitedAsync OnAfterRenderAsync");
            }



            StateHasChanged();
        }
    }

    /// <summary>
    /// Update the text of last breadcrumb
    /// </summary>
    /// <param name="text"></param>
    /// <example>
    /// UpdateLastBreadcrumb("New Text");
    /// </example>
    public void ReplaceLastBreadcrumb(string text)
    {
        if (BreadcrumbsComp is null)
        {
            _breadcrumbSetCallback = () => BreadcrumbsComp!.ReplaceLastBreadcrumb(text);
        }
        else
        {
            BreadcrumbsComp.ReplaceLastBreadcrumb(text);
        }
    }

    /// <summary>
    /// Update breadcrumbs
    /// </summary>
    /// <param name="configure"></param>
    /// <example>
    /// UpdateBreadcrumbs(items => {
    ///     items[0].Text = "New Text";
    ///     items[0].Url = "/new-url";
    /// })
    /// </example>
    public void UpdateBreadcrumbs(Action<List<BreadcrumbItem>> configure)
    {
        if (BreadcrumbsComp is null)
        {
            _breadcrumbSetCallback = () => BreadcrumbsComp!.UpdateBreadcrumbs(configure);
        }
        else
        {
            BreadcrumbsComp.UpdateBreadcrumbs(configure);
        }
    }

    private bool IsMenusUri(List<Nav> navs, string uri)
    {
        if (_whiteUriList.Any(w => Regex.IsMatch(uri.ToLower(), w, RegexOptions.IgnoreCase)))
        {
            return true;
        }

        var allowed = navs.Any(n => (n.Url ?? "").Equals(uri, StringComparison.OrdinalIgnoreCase));
        if (!allowed)
        {
            foreach (var nav in navs)
            {
                if (nav.HasChildren)
                {
                    allowed = IsMenusUri(nav.Children, uri);
                }

                if (allowed)
                {
                    break;
                }
            }
        }

        return allowed;
    }

    private List<Nav> FlattenNavs(List<Nav> tree, bool excludeNavHasChildren = false)
    {
        var res = new List<Nav>();

        foreach (var nav in tree)
        {
            if (!(nav.HasChildren && excludeNavHasChildren))
            {
                if (nav.HasChildren)
                {
                    nav.ChildUrl = nav.Children.FirstOrDefault(u => !u.HasChildren)?.Url;
                }

                res.Add(nav);
            }

            if (nav.HasChildren)
            {
                foreach (var child in nav.Children)
                {
                    child.ParentCode = nav.Code;
                }

                res.AddRange(FlattenNavs(nav.Children, excludeNavHasChildren));
            }
        }

        return res;
    }

    protected override void OnInitialized()
    {
        OnErrorAsync ??= async exception => { await PopupService.EnqueueSnackbarAsync(exception, false); };

        ErrorContent ??= Exception => builder => { };

        NavigationManager.LocationChanged += HandleLocationChanged;

        I18nCache.OnSectionUpdated += HandleSectionUpdated;
    }

    private void HandleSectionUpdated()
    {
        InvokeAsync(StateHasChanged); 
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var absolutePath = NavigationManager.GetAbsolutePath();

        if (!CheckAuthenticated())
        {
            return;
        }

        if (absolutePath.Contains("/dashboard") is false && !IsMenusUri(NavItems, absolutePath))
        {
            NavigationManager.NavigateTo("/403");
            return;
        }

        Logger.LogInformation("URL of new location: {Location}", e.Location);
        try
        {
            AuthClient.UserService.VisitedAsync(AppId, absolutePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "AuthClient.UserService.VisitedAsync");
        }
    }

    private bool CheckAuthenticated()
    {
        var absolutePath = NavigationManager.GetAbsolutePath();

        if (absolutePath.Contains("/authentication"))
        {
            return false;
        }

        var authState = AuthenticationStateTask.Result;
        if (authState.User.Identity?.IsAuthenticated != true)
        {
            NavigationManager.NavigateTo("/authentication/login");
            return false;
        }

        return true;
    }

    private async Task AddFavoriteMenu(string code)
    {
        await AuthClient.PermissionService.AddFavoriteMenuAsync(Guid.Parse(code));
    }

    private async Task RemoveFavoriteMenu(string code)
    {
        await AuthClient.PermissionService.RemoveFavoriteMenuAsync(Guid.Parse(code));
    }

    private async Task<bool> OnErrorHandleAsync(Exception exception)
    {
        if (OnErrorAsync != null)
        {
            await OnErrorAsync.Invoke(exception);
        }

        return true;
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }
}
