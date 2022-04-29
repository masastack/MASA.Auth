namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class UserClaimSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public List<int> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<int>> ValueChanged { get; set; }
}

