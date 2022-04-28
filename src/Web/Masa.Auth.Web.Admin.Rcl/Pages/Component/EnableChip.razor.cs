namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class EnableChip
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public bool Value { get; set; }

    public string Color => Value ? "sample-green" : "error";
}

