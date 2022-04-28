using Masa.Auth.ApiGateways.Caller;

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public abstract class AdminCompontentBase : ComponentBase
{
    private I18n? _i18n;
    private GlobalConfig? _globalConfig;
    private AuthCaller? _authCaller;
    private NavigationManager? _navigationManager;

    [Inject]
    public AuthCaller AuthCaller
    {
        get
        {
            return _authCaller ?? throw new Exception("please Inject AuthCaller!");
        }
        set
        {
            _authCaller = value;
        }
    }

    [Inject]
    public I18n I18n
    {
        get
        {
            return _i18n ?? throw new Exception("please Inject I18n!");
        }
        set
        {
            _i18n = value;
        }
    }

    [Inject]
    public GlobalConfig GlobalConfig
    {
        get
        {
            return _globalConfig ?? throw new Exception("please Inject GlobalConfig!");
        }
        set
        {
            _globalConfig = value;
        }
    }

    [Inject]
    public NavigationManager NavigationManager
    {
        get
        {
            return _navigationManager ?? throw new Exception("please Inject NavigationManager!");
        }
        set
        {
            _navigationManager = value;
        }

    }

    [Inject]
    public IPopupService PopupService { get; set; } = default!;

    public bool Loading
    {
        get => GlobalConfig.Loading;
        set => GlobalConfig.Loading = value;
    }

    public string T(string key) => I18n.T(key);

    public async Task<bool> OpenConfirmDialog(string content)
    {
        return await PopupService.ConfirmAsync(T("Operation confirmation"), content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content)
    {
        return await PopupService.ConfirmAsync(title, content);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content, AlertTypes type)
    {
        return await PopupService.ConfirmAsync(title, content, type);
    }

    public void OpenInformationMessage(string message)
    {
        PopupService.AlertAsync(message, AlertTypes.Info);
    }

    public void OpenSuccessMessage(string message)
    {
        PopupService.AlertAsync(message, AlertTypes.Success);
    }

    public void OpenWarningMessage(string message)
    {
        PopupService.AlertAsync(message, AlertTypes.Warning);
    }

    public void OpenErrorMessage(string message)
    {
        PopupService.AlertAsync(message, AlertTypes.Error);
    }

    public List<KeyValuePair<string, TEnum>> GetEnumMap<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>().Select(e => new KeyValuePair<string, TEnum>(e.ToString(), e)).ToList();
    }

    public List<KeyValuePair<string, bool>> GetBooleanMap()
    {
        return new()
        {
            new(T("Enable"), true),
            new(T("Disabled"), false)
        };
    }
}

