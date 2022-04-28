namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class ButtonGroup<TValue> where TValue : struct, Enum
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public TValue Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public StyleTypes StyleType { get; set; }

    public List<KeyValuePair<string, TValue>> KeyValues { get; set; } = new();

    protected override void OnInitialized()
    {
        KeyValues = GetEnumMap<TValue>();
    }
}

