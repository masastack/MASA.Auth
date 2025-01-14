namespace Masa.Stack.Components;

[Obsolete("Use SBtn instead")]
public class SAutoLoadingButton : MButton
{
    [Parameter] public string BorderRadiusClass { get; set; } = "rounded-pill";

    [Parameter] public bool DisableLoading { get; set; }

    private CancellationTokenSource? _cancellationTokenSource;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Color = "primary";
        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Class ??= "";
        Class += " " + BorderRadiusClass;

        if (OnClick.HasDelegate)
        {
            var originalOnClick = OnClick;

            OnClick = EventCallback.Factory.Create<MouseEventArgs>(this, async (args) =>
            {
                Loading = DisableLoading is false;
                Disabled = true;
                StateHasChanged();

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    await Task.Delay(100, _cancellationTokenSource.Token);
                    await originalOnClick.InvokeAsync(args);
                }
                catch (TaskCanceledException)
                {
                    // ignored
                }
                finally
                {
                    Loading = false;
                    Disabled = false;
                    StateHasChanged();
                }
            });
        }
    }
}