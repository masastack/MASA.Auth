namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso;

public partial class UpdateClient
{
    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }


}
