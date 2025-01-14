namespace Masa.Stack.Components;

public partial class SIcon : MIcon
{
    [Parameter]
    public string? Tooltip { get; set; }

    [Inject]
    private I18n? I18N { get; set; }

    private static readonly Dictionary<string, string> IconI18N = new()
    {
        { "mdi-pencil" , "Edit" },
        { "mdi-check" , "Save" },
        { "mdi-pin" , "Pin" },
        { "mdi-magnify" , "Search" },
        { "mdi-dots-horizontal" , "More" },
        { "mdi-dots-vertical" , "More" },
        { "far fa-copy" , "Copy" },
        { "mdi-chevron-down" , "Expand" },
        { "mdi-plus" , "Add" },
        { "mdi-delete" , "Delete" },
        { "mdi-link-variant" , "Relevance" },
        { "mdi-close" , "Close" },
        { "mdi-keyboard-backspace", "PreviousStep" },
        { "mdi-chevron-left", "PreviousStep"},
        { "mdi-chevron-right", "NextStep"},
        { "mdi-star","Favorite" },
        { "mdi-star-outline","CancelFavorite" },
    };

    [Parameter]
    public bool IsDefaultToolTip { get; set; } = true;

    CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Size = 20;
        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        var originalOnClick = OnClick;

        if (OnClick.HasDelegate)
        {
            OnClick = EventCallback.Factory.Create<MouseEventArgs>(this, async (args) =>
            {
                Disabled = true;

                try
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = new CancellationTokenSource();
                    await Task.Delay(500, _cancellationTokenSource.Token);

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    await originalOnClick.InvokeAsync(args);
                }
                finally
                {
                    Disabled = false;
                    StateHasChanged();
                }
            });
        }
    }

    private void InitDefaultToolTip()
    {
        if (IsDefaultToolTip && Tooltip is null && IconContent is not null)
        {
            var icon = IconContent.Trim();
            if (IconI18N.TryGetValue(icon, out string? value))
            {
                Tooltip = I18N?.T(value);
            }
            else
            {
                value = IconI18N.FirstOrDefault(x => icon.Contains(x.Key)).Value;
                if (value is not null)
                {
                    Tooltip = I18N?.T(value);
                }
            }
        }
    }
}
