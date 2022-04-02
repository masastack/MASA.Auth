namespace Masa.Auth.Web.Admin.Rcl.Shared.Presets;

public class ElevationTabItem : ComponentBase, IDisposable
{
    [CascadingParameter]
    private ElevationTab? Tab { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        Tab?.AddTabItem(this);
        base.OnInitialized();
    }

    public void Dispose()
    {
        Tab?.RemoveTabItem(this);
    }
}
