namespace Masa.Stack.Components;

public class STextarea : MTextarea
{
    private RenderFragment? _requiredLabelContent;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        HideDetails = "auto";
        Outlined = true;

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Required && LabelContent == default)
        {
            LabelContent = _requiredLabelContent ??= RenderFragments.GenRequiredLabel(Label);
        }
    }
}