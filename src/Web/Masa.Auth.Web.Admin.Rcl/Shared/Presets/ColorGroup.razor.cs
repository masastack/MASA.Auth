namespace Masa.Auth.Web.Admin.Rcl.Shared.Presets;

public partial class ColorGroup
{
    [Parameter]
    public List<string> Colors { get; set; } = new();

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public int Size { get; set; } = 24;

    [Parameter]
    public bool SpaceBetween { get; set; } = false;

    protected override void OnParametersSet()
    {
        if (Colors.Any())
        {
            Value = Colors.First();
        }
        base.OnParametersSet();
    }
}
