namespace Masa.Auth.Web.Admin.Rcl.Shared.Presets;

public partial class ClickedExtend
{
    string displayNone = "display:none !important;", displayFlex = "display:flex;";

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? ExtendContent { get; set; }

    public bool IsExpanded { get; set; }

    [Parameter]
    public bool Split { get; set; }
    
    [Parameter]
    public StringNumber MaxWidth { get; set; } = "100%";

    [Parameter]
    public string TriggerIcon { get; set; } = "mdi-magnify";

    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    [Parameter]
    public EventCallback OnBlur { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    private async Task OnBlurHandler()
    {
        IsExpanded = false;
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync();
        }
    }
}
