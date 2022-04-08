namespace Masa.Auth.Web.Admin.Rcl.Shared.Default;

public class DefaultTextField<TValue> : MTextField<TValue>
{
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = true;
        Dense = true;
        HideDetails = false;
        Outlined = true;

        await base.SetParametersAsync(parameters);
    }
}
