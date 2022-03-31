using Masa.Auth.Contracts.Admin.Infrastructure.Enums;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class UpdateSheet
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public TeamDetailDto Dto { get; set; } = new();

    [Parameter]
    public EventCallback<UpdateTeamBaseInfoDto> OnUpdateBase { get; set; }

    [Parameter]
    public EventCallback<UpdateTeamPersonnelDto> OnUpdateAdmin { get; set; }

    [Parameter]
    public EventCallback<UpdateTeamPersonnelDto> OnUpdateMember { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    StringNumber _tab = 1;

    protected override void OnInitialized()
    {
        Dto.TeamBaseInfo.PropertyChanged += (sender, args) => StateHasChanged();
    }

    public void Dispose()
    {
        Dto.TeamBaseInfo.PropertyChanged -= (sender, args) => StateHasChanged();
    }

    public async Task OnUpdateBaseHandler()
    {
        if (OnUpdateBase.HasDelegate)
        {
            await OnUpdateBase.InvokeAsync(new UpdateTeamBaseInfoDto
            {
                Id = Dto.Id,
                Name = Dto.TeamBaseInfo.Name,
                Description = Dto.TeamBaseInfo.Description,
                Type = (TeamTypes)Dto.TeamBaseInfo.Type,
                Avatar = Dto.TeamBaseInfo.Avatar
            });
        }
        await ShowChanged.InvokeAsync(false);
    }

    public async Task OnUpdateAdminHandler()
    {
        if (OnUpdateAdmin.HasDelegate)
        {
            await OnUpdateAdmin.InvokeAsync(new UpdateTeamPersonnelDto
            {
                Id = Dto.Id,
                Staffs = Dto.TeamAdmin.Staffs,
                Roles = Dto.TeamAdmin.Roles,
                Permissions = Dto.TeamAdmin.Permissions
            });
        }
        await ShowChanged.InvokeAsync(false);
    }

    public async Task OnUpdateMemberHandler()
    {
        if (OnUpdateMember.HasDelegate)
        {
            await OnUpdateMember.InvokeAsync(new UpdateTeamPersonnelDto
            {
                Id = Dto.Id,
                Staffs = Dto.TeamMember.Staffs,
                Roles = Dto.TeamMember.Roles,
                Permissions = Dto.TeamMember.Permissions
            });
        }
        await ShowChanged.InvokeAsync(false);
    }

    public async Task OnDeleteHandler()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(Dto.Id);
        }
        await ShowChanged.InvokeAsync(false);
    }
}
