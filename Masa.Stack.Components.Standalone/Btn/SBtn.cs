namespace Masa.Stack.Components.Standalone;

public class SBtn : MButton
{
    [Parameter] public bool AutoLoading { get; set; }

    private CancellationTokenSource? _cancellationTokenSource;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Assume AutoLoading will not change
        if (AutoLoading && OnClick.HasDelegate)
        {
            var originalOnClick = OnClick;

            OnClick = EventCallback.Factory.Create<MouseEventArgs>(this, async (args) =>
            {
                Loading = true;
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