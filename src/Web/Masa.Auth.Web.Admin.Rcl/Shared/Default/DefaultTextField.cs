namespace Masa.Auth.Web.Admin.Rcl.Shared.Default;

public class DefaultTextField<TValue> : MTextField<TValue>
{
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = true;
        Dense = true;
        HideDetails = "auto";
        Outlined = true;

        await base.SetParametersAsync(parameters);
    }
}
