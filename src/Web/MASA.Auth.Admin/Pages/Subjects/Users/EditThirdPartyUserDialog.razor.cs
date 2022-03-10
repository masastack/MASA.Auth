namespace Masa.Auth.Admin.Pages.Subjects.Users;

public partial class EditThirdPartyUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyUserUserId { get; set; } 

    private bool IsAdd => ThirdPartyUserUserId == Guid.Empty;

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if(Visible is true)
        {
            
        }
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

