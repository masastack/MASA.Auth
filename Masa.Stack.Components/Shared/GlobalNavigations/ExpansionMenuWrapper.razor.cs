namespace Masa.Stack.Components.Shared.GlobalNavigations;

public partial class ExpansionMenuWrapper : MasaComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public ExpansionMenu? Value { get; set; }

    [Parameter]
    public bool RenderLayer { get; set; }

    [Parameter]
    public EventCallback<ExpansionMenu> OnItemClick { get; set; }

    [Parameter]
    public EventCallback<ExpansionMenu> OnItemOperClick { get; set; }

    [Parameter]
    public string? CssForScroll { get; set; }

    private readonly string idPrefix = "g" + Guid.NewGuid().ToString();

    private string CssSelectorForScroll => string.IsNullOrWhiteSpace(CssForScroll) ? string.Empty : "." + CssForScroll;

    private bool _shouldUpdateMasonry;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldUpdateMasonry)
        {
            _shouldUpdateMasonry = false;
            await InitMasonryAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        _shouldUpdateMasonry = true;
        await base.OnParametersSetAsync();
    }

    protected virtual async Task InitMasonryAsync()
    {
        if (Value is not null)
        {
            foreach (var category in Value.Children)
            {
                await JsRuntime.InvokeVoidAsync("MasaStackComponents.initMasonry", $".{idPrefix}_{category.Id}", ".app", 24);
            }
        }
    }

    protected virtual async Task ItemClick(ExpansionMenu menu)
    {
        if (Value.MetaData.Situation == ExpansionMenuSituation.Authorization)
        {
            await menu.ChangeStateAsync();
        }

        if (OnItemClick.HasDelegate)
        {
            await OnItemClick.InvokeAsync(menu);
        }
    }

    protected virtual async Task ItemOperClick(ExpansionMenu menu)
    {
        await menu.ChangeStateAsync();

        if (OnItemOperClick.HasDelegate)
        {
            await OnItemOperClick.InvokeAsync(menu);
        }
    }

    private async Task ScrollTo(string tagId, string insideSelector)
    {
        await JsRuntime.InvokeVoidAsync("MasaStackComponents.scrollTo", $"#{tagId}", insideSelector);
    }
}
