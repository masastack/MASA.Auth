namespace Masa.Auth.Web.Admin.Rcl.Shared.Default;

public class DefaultNumberTextField<TValue> : MTextField<TValue>
{
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = false;
        Dense = true;
        HideDetails = false;
        Outlined = true;
        Type = "number";

        await base.SetParametersAsync(parameters);
    }
}
