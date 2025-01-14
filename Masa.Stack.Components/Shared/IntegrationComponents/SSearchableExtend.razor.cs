namespace Masa.Stack.Components;

public partial class SSearchableExtend
{
    readonly string _displayNone = "display:none !important;";
    readonly string _displayFlex = "display:flex;";

    bool IsExpanded { get; set; }

    string CardStyle => IsExpanded ? _displayFlex : _displayNone;

    string ChildStyle => IsExpanded ? _displayNone : _displayFlex;

    [Inject]
    public I18n I18n { get; set; } = null!;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Split { get; set; }

    [Parameter]
    public StringNumber MaxWidth { get; set; } = "100%";

    [Parameter]
    public string TriggerIcon { get; set; } = "mdi-magnify";

    [Parameter]
    public EventCallback<string> OnEnter { get; set; }

    [Parameter]
    public EventCallback OnBlur { get; set; }

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    private async Task OnBlurHandler()
    {
        IsExpanded = false;
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync();
        }
    }

    private async Task OnKeyDownHandler(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "Enter")
        {
            if (OnEnter.HasDelegate)
            {
                await OnEnter.InvokeAsync(Value);
            }
        }
    }
}
