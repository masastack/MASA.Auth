namespace Masa.Stack.Components;

public partial class SConfirm
{
    private string? _icon;
    private string? _iconColor;

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Icon
    {
        get
        {
            return string.IsNullOrEmpty(_icon)
                ? Type switch
                {
                    AlertTypes.Success => "mdi-checkbox-marked-circle-outline",
                    AlertTypes.Error => "mdi-alert-circle-outline",
                    AlertTypes.Info => "mdi-information",
                    AlertTypes.Warning => "mdi-alert-outline",
                    _ => null,
                }
                : _icon;
        }
        set { _icon = value; }
    }

    [Parameter]
    [NotNull]
    public string? IconColor
    {
        get
        {
            return string.IsNullOrEmpty(_iconColor)
                ? Type switch
                {
                    AlertTypes.Success => "success",
                    AlertTypes.Info => "info",
                    AlertTypes.Warning => "warning",
                    AlertTypes.Error => "error",
                    _ => "primary"
                }
                : _iconColor;
        }
        set { _iconColor = value; }
    }

    [Parameter]
    public AlertTypes Type { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnOk { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnCancel { get; set; }

    [Parameter]
    public RenderFragment? ActionContent { get; set; }

    [Parameter]
    public RenderFragment<Func<MouseEventArgs, Task>>? OkContent { get; set; }

    [Parameter]
    public RenderFragment<Func<MouseEventArgs, Task>>? CancelContent { get; set; }

    private async Task HandleOnCancel(MouseEventArgs args)
    {
        await OnCancel.InvokeAsync(args);
        await UpdaetVisibleAsync();
    }

    private async Task HandleOnOk(MouseEventArgs args)
    {
        await OnOk.InvokeAsync(args);
    }

    async Task UpdaetVisibleAsync()
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(false);
        }
        else Visible = false;
    }
}