namespace Masa.Stack.Components;

public partial class SItemCol : MasaComponentBase
{
    [Inject]
    public JsInitVariables JsInitVariables { get; set; } = default!;

    [Parameter]
    public Func<bool, string>? BoolFormatter { get; set; }

    [Parameter]
    public string? BoolClass { get; set; }

    [Parameter]
    public bool ChippedEnum { get; set; }

    [Parameter]
    public bool SmallChip { get; set; }

    [Parameter]
    public Func<Enum, string>? EnumColorFormatter { get; set; }

    [Parameter]
    public string? EnumClass { get; set; }

    [Parameter]
    public Func<DateTime, bool>? DateTimeRule { get; set; }

    [Parameter]
    public bool IgnoreTime { get; set; }

    [Parameter]
    public string? DateFormat { get; set; }

    [Parameter]
    public string? TimeFormat { get; set; }

    [Parameter]
    public string? DateTimeClass { get; set; }

    [Parameter, EditorRequired]
    public object? Value { get; set; }

    [Parameter]
    public string? DefaultClass { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        BoolFormatter ??= b => b ? T("Yes") : T("No");
        DateTimeRule ??= dateTime => dateTime != default;
    }

    public string GetColor(Enum @enum)
    {
        if (EnumColorFormatter is not null)
        {
            return EnumColorFormatter.Invoke(@enum);
        }

        return ColorHelper.GetColor((int)(object)@enum);
    }
}
