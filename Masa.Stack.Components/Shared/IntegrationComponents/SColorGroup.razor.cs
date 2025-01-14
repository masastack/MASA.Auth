namespace Masa.Stack.Components;

public partial class SColorGroup
{
    [EditorRequired]
    [Parameter]
    public List<string> Colors { get; set; } = new();

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public int Size { get; set; } = 24;

    [Parameter]
    public bool SpaceBetween { get; set; } = false;

    [Parameter]
    public RenderFragment<string>? ItemAppendContent { get; set; }

    [Parameter]
    public int Elevation { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Colors.Any() && Value == string.Empty)
            {
                await ValueChanged.InvokeAsync(Colors.First());
            }
            else
            {
                await ValueChanged.InvokeAsync(Value);
            }

            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnClickHandler(ItemContext context, string color)
    {
        await context.Toggle();
        await ValueChanged.InvokeAsync(color);
    }

    private async Task HandleValueChanged(StringNumber color)
    {
        await ValueChanged.InvokeAsync(color.AsT0);
    }
}
