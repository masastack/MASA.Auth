namespace Masa.Stack.Components;

public class EnumSimpleSelect<TValue> : SSimpleSelect<TValue> where TValue : struct, Enum
{
    [Parameter]
    public override List<(TValue value, string text)> ValueTexts
    {
        get
        {
            var valueTexts = Enum.GetValues<TValue>()
                        .Select(e => (e, ConvertText(e)))
                        .ToList();

            return valueTexts;
        }
        set
        {

        }
    }

    [Parameter]
    public Func<TValue, string> ConvertText { get; set; } = default!;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        ConvertText ??= value => T(value.ToString());
    }
}
