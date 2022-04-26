namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class Record
{
    [Parameter]
    public string Creator { get; set; } = "";

    [Parameter]
    public DateTime CreationTime { get; set; }

    [Parameter]
    public string Modifier { get; set; } = "";

    [Parameter]
    public DateTime? ModificationTime { get; set; }
}
