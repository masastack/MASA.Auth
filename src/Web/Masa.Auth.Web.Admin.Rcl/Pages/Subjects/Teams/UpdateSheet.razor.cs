// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class UpdateSheet
{
    [Parameter]
    public TeamDetailDto Dto { get; set; } = new();

    [Parameter]
    public EventCallback<UpdateTeamDto> OnUpdate { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    bool _adminPreview, _memberPreview, _visible;
    string _tab = "";
    List<string> _tabs = new();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _tabs = new List<string> { T("Basic Information"), T("Setup team admins"), T("Setup team members") };
        }
        _tab = T("Basic Information");
        base.OnAfterRender(firstRender);
    }

    public void Show(TeamDetailDto model)
    {
        Dto = model;
        _visible = true;
    }

    private async Task OnUpdateHandler()
    {
        if (OnUpdate.HasDelegate)
        {
            await OnUpdate.InvokeAsync(new UpdateTeamDto
            {
                Id = Dto.Id,
                Name = Dto.TeamBasicInfo.Name,
                Description = Dto.TeamBasicInfo.Description,
                Type = (TeamTypes)Dto.TeamBasicInfo.Type,
                Avatar = Dto.TeamBasicInfo.Avatar,
                AdminStaffs = Dto.TeamAdmin.Staffs,
                AdminRoles = Dto.TeamAdmin.Roles,
                AdminPermissions = Dto.TeamAdmin.Permissions,
                MemberStaffs = Dto.TeamMember.Staffs,
                MemberRoles = Dto.TeamMember.Roles,
                MemberPermissions = Dto.TeamMember.Permissions
            });
        }
        _visible = false;
    }

    private async Task OnDeleteHandler()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(Dto.Id);
        }
        _visible = false;
    }
}
