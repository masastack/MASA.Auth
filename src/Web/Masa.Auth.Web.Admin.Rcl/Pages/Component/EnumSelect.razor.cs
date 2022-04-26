namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class EnumSelect<TValue> where TValue : struct, Enum
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Placeholder { get; set; } = "";

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public TValue Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    public List<KeyValuePair<string, TValue>> KeyValues { get; set; } = new ();

    protected override void OnInitialized()
    {
        KeyValues = GetEnumMap<TValue>();
    }
}

