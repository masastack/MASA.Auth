namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class BooleanSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Placeholder { get; set; } = "";

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public bool? Value { get; set; }

    [Parameter]
    public EventCallback<bool?> ValueChanged { get; set; }

    public List<KeyValuePair<string, bool>> KeyValues { get; set; } = new ();

    protected override void OnInitialized()
    {
        KeyValues = GetBooleanMap();
    }
}

