namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class ApiScopeSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public List<int> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<int>> ValueChanged { get; set; }
}

