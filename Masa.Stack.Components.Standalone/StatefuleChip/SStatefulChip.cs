// ReSharper disable StaticMemberInGenericType

namespace Masa.Stack.Components.Standalone;

public class SStatefulChip<TState> : MChip
{
    [Parameter] public TState? SuccessState { get; set; }

    [Parameter] public TState? ErrorState { get; set; }

    [Parameter] public TState? WarningState { get; set; }

    [Parameter] public TState? InfoState { get; set; }

    [Parameter] public TState? NeutralState { get; set; }

    [Parameter] public TState? PremiumState { get; set; }

    [Parameter] public TState? State { get; set; }

    /// <summary>
    /// Rule to determine the color of the chip.
    /// Accepts built-in states: "info", "success", "error", "warning", "neutral", "premium"
    /// and custom built-in colors.
    /// </summary>
    [Parameter]
    public Func<TState, string>? Rule { get; set; }

    private static string _successColorCss = GetColorCss("green");
    private static string _errorColorCss = GetColorCss("red");
    private static string _warningColorCss = GetColorCss("orange");
    private static string _infoColorCss = GetColorCss("blue");
    private static string _neutralColorCss = GetColorCss("grey");
    private static string _premiumColorCss = GetColorCss("purple");

    private TState? _prevState;
    private bool _firstSet = true;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_firstSet || !EqualityComparer<TState>.Default.Equals(_prevState, State))
        {
            _firstSet = false;
            _prevState = State;

            if (!IsDirtyParameter(nameof(Color)) && State is not null)
            {
                Color = Rule is not null ? GetStateCss(Rule(State)) : GetDefaultRuleStateCss();
            }
        }
    }

    private string? GetDefaultRuleStateCss()
    {
        if (SuccessState != null && EqualityComparer<TState>.Default.Equals(SuccessState, State))
        {
            return _successColorCss;
        }

        if (ErrorState != null && EqualityComparer<TState>.Default.Equals(ErrorState, State))
        {
            return _errorColorCss;
        }

        if (WarningState != null && EqualityComparer<TState>.Default.Equals(WarningState, State))
        {
            return _warningColorCss;
        }

        if (InfoState != null && EqualityComparer<TState>.Default.Equals(InfoState, State))
        {
            return _infoColorCss;
        }

        if (NeutralState != null && EqualityComparer<TState>.Default.Equals(NeutralState, State))
        {
            return _neutralColorCss;
        }

        if (PremiumState != null && EqualityComparer<TState>.Default.Equals(PremiumState, State))
        {
            return _premiumColorCss;
        }

        return null;
    }

    private static string? GetStateCss(string? stateOrColor)
    {
        if (string.IsNullOrWhiteSpace(stateOrColor))
        {
            return null;
        }

        var color = stateOrColor switch
        {
            "info" => "blue",
            "success" => "green",
            "error" => "red",
            "warning" => "orange",
            "neutral" => "grey",
            "premium" => "purple",
            _ => stateOrColor
        };

        return GetColorCss(color);
    }

    private static string GetColorCss(string color)
    {
        return string.Format("{0} lighten-5 {0}--text", color);
    }
}