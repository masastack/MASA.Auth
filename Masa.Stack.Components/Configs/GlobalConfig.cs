namespace Masa.Stack.Components.Configs;

public class GlobalConfig : IScopedDependency
{
    private const string DarkStoreKey = "GlobalConfig_IsDark";
    private const string MiniStoreKey = "GlobalConfig_NavigationMini";
    private const string FavoriteStoreKey = "GlobalConfig_Favorite";
    private const string LangStoreKey = "GlobalConfig_Lang";
    private const string NavLayerStoreKey = "GlobalConfig_NavLayer";

    private readonly LocalStorage _localStore;
    private readonly I18n _i18N;
    private bool _dark;
    private bool _mini;
    private string _favorite;
    private Guid _currentTeamId;
    private bool _loading;
    private int _navLayer = 2;

    public List<int> NavLayerItems = new List<int> { 1, 2, 3 };

    public string LoadingText = string.Empty;

    public delegate void GlobalConfigChanged();

    public event GlobalConfigChanged? OnLanguageChanged;

    public delegate void CurrentTeamChanged(Guid teamId);

    public event CurrentTeamChanged? OnCurrentTeamChanged;

    public delegate Task NavLayerChanged();

    public event NavLayerChanged? OnNavLayerChanged;

    public delegate void LoadingChanged(bool loading, string loadingText);

    public event LoadingChanged? OnLoadingChanged;

    public GlobalConfig(I18n i18n, LocalStorage localStore)
    {
        _i18N = i18n;
        _localStore = localStore;
    }

    public CultureInfo Culture
    {
        get => _i18N.Culture;
        set
        {
            _i18N.SetCulture(new CultureInfo("en-US"), value);
            _localStore.SetItemAsync(LangStoreKey, value.Name);
            OnLanguageChanged?.Invoke();
        }
    }

    public Guid CurrentTeamId
    {
        get
        {
            return _currentTeamId;
        }
        set
        {
            if (_currentTeamId != value)
            {
                _currentTeamId = value;
                OnCurrentTeamChanged?.Invoke(value);
            }
        }
    }

    public bool Dark
    {
        get => _dark;
        set
        {
            _dark = value;
            _localStore.SetItemAsync(DarkStoreKey, value);
        }
    }

    public List<Nav> Menus { get; set; }

    public bool Mini
    {
        get => _mini;
        set
        {
            _mini = value;
            _localStore?.SetItemAsync(MiniStoreKey, value);
        }
    }

    public string Favorite
    {
        get => _favorite;
        set
        {
            _favorite = value;
            _localStore?.SetItemAsync(FavoriteStoreKey, value);
        }
    }

    public int NavLayer
    {
        get => _navLayer;
        set
        {
            if (_navLayer != value)
            {
                _navLayer = value;
                OnNavLayerChanged?.Invoke();
                _localStore.SetItemAsync(NavLayerStoreKey, value);
            }
        }
    }

    public bool Loading
    {
        get => _loading;
        set
        {
            if (_loading != value)
            {
                _loading = value;
                OnLoadingChanged?.Invoke(_loading, LoadingText);
            }
        }
    }

    public async void Initialization()
    {
        _dark = Convert.ToBoolean(await _localStore.GetItemAsync(DarkStoreKey));
        bool.TryParse(await _localStore.GetItemAsync(MiniStoreKey), out _mini);
        _favorite = await _localStore.GetItemAsync(FavoriteStoreKey) ?? string.Empty;

        var lang = await _localStore.GetItemAsync(LangStoreKey);
        if (!string.IsNullOrWhiteSpace(lang))
        {
            _i18N.SetCulture(new CultureInfo("en-US"), new CultureInfo(lang));
        }

        var navLayer = await _localStore.GetItemAsync<int>(NavLayerStoreKey);
        if (navLayer > 0)
        {
            _navLayer = navLayer;
        }
    }
}
