namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class ButtonGroup<TValue> where TValue : struct
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public TValue Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public IEnumerable<TValue> Values { get; set; } = new List<TValue>();

    [Parameter]
    public StyleTypes StyleType { get; set; }
}

