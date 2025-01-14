namespace Masa.Stack.Components;

public partial class SPaginationSelect
{
    [Parameter]
    public int Value { get; set; }

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    [Parameter]
    public List<int> Items { get; set; } = new();

    public bool MenuState { get; set; }

    private string Icon => MenuState ? "mdi-menu-up" : "mdi-menu-down";

    public async Task SelectAsync(int value)
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else
        {
            Value = value;
        }
        //Icon = "mdi-menu-down";
    }

    //public void ChangeIconState(bool show)
    //{
    //    if (show) Icon = "mdi-menu-up";
    //    else Icon = "mdi-menu-down";
    //}
}

