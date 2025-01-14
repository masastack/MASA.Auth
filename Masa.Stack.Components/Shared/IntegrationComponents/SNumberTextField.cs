namespace Masa.Stack.Components;

public class SNumberTextField<TValue> : STextField<TValue>
{
    [Parameter]
    public int Min { get; set; } = 0;

    public new int Max { get; set; } = int.MaxValue;

    public int Step { get; set; } = 1;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = false;
        Type = "number";
        NumberProps = (prop) =>
        {
            prop.Min = Min;
            prop.Step = Step;
            prop.Max = Max;
        };
        await base.SetParametersAsync(parameters);
    }
}
