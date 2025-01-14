
namespace Masa.Stack.Components;

public abstract class MasaComponentBase : NextTickComponentBase
{
    [Inject]
    public I18n I18n { get; set; } = null!;

    [CascadingParameter(Name = "Culture")]
    private string Culture { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IAuthClient AuthClient { get; set; } = null!;

    [Inject]
    public IPmClient PmClient { get; set; } = null!;

    [Inject]
    public MasaUser MasaUser { get; set; } = null!;

    [Inject]
    public IPopupService PopupService { get; set; } = default!;

    [Inject]
    public IMasaStackConfig MasaStackConfig { get; set; } = default!;

    [Inject]
    public DynamicTranslateProvider TranslateProvider { get; set; } = default!;

    [Parameter] public string? Class { get; set; }

    [Parameter] public string? Style { get; set; }

    protected string T(string key)
    {
        return I18n.T(key);
    }

    protected string T(string key, params object[] args)
    {
        return I18n.T(key, args: args);
    }

    protected string T(string scope, string key, params object[] args)
    {
        return I18n.T(scope, key, args: args);
    }

    protected string DT(string key)
    {
        return TranslateProvider.DT(key);
    }

    protected const string DateTimeRangeScope= "DateTimeRange";

    protected string GetIsDisplayStyle(bool show)
    {
        return show ? "" : "display:none !important;";
    }
}