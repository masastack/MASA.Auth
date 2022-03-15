using Masa.Auth.ApiGateways.Caller;

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public abstract class AdminCompontentBase : ComponentBase
{
    private I18n? _i18n;
    private GlobalConfig? _globalConfig;
    private AuthClient? _authClient;

    [Inject]
    public AuthClient AuthClient
    {
        get
        {
            return _authClient ?? throw new Exception("please Inject AuthClient!");
        }
        set
        {
            _authClient = value;
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

    public bool Loading
    {
        get => GlobalConfig.Loading;
        set => GlobalConfig.Loading = value;
    }

    public string T(string key) => I18n.T(key);

    public void OpenConfirmDialog(Func<bool, Task> confirmFunc, string messgae)
    {
        var callback = EventCallback.Factory.Create(this, confirmFunc);
        GlobalConfig.OpenConfirmDialog(I18n.T("Operation confirmation"), messgae, callback);
    }

    public void OpenErrorDialog(string message)
    {
        GlobalConfig.OpenConfirmDialog(I18n.T("Error"), message, default);
    }

    public void OpenWarningDialog(string message)
    {
        GlobalConfig.OpenConfirmDialog(I18n.T("Warning"), message, default);
    }

    public void OpenInformationMessage(string message)
    {
        GlobalConfig.OpenMessage(message, MessageType.Information);
    }

    public void OpenSuccessMessage(string message)
    {
        GlobalConfig.OpenMessage(message, MessageType.Success);
    }

    public void OpenWarningMessage(string message)
    {
        GlobalConfig.OpenMessage(message, MessageType.Warning);
    }

    public void OpenErrorMessage(string message)
    {
        GlobalConfig.OpenMessage(message, MessageType.Error);
    }
}

