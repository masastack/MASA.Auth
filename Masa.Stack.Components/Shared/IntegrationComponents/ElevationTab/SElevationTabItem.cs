namespace Masa.Stack.Components;

public class SElevationTabItem : ComponentBase, IDisposable
{
    [CascadingParameter]
    private SElevationTab? Tab { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private int? _index;

    protected override void OnInitialized()
    {
        _index = Tab?.AddTabItem(this);
        base.OnInitialized();
    }

    protected override bool ShouldRender()
    {
        var shouldRender = _index == Tab?.TabIndex;
        return shouldRender;
    }

    public void Dispose()
    {
        Tab?.RemoveTabItem(this);
    }
}
