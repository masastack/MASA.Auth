// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class UpdateSheet
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [EditorRequired]
    [Parameter]
    public TeamDetailDto Dto { get; set; } = new();

    [Parameter]
    public EventCallback<UpdateTeamBasicInfoDto> OnUpdateBase { get; set; }

    [Parameter]
    public EventCallback<UpdateTeamPersonnelDto> OnUpdateAdmin { get; set; }

    [Parameter]
    public EventCallback<UpdateTeamPersonnelDto> OnUpdateMember { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    bool _adminPreview, _memberPreview;

    public async Task OnUpdateBaseHandler()
    {
        if (OnUpdateBase.HasDelegate)
        {
            await OnUpdateBase.InvokeAsync(new UpdateTeamBasicInfoDto
            {
                Id = Dto.Id,
                Name = Dto.TeamBasicInfo.Name,
                Description = Dto.TeamBasicInfo.Description,
                Type = (TeamTypes)Dto.TeamBasicInfo.Type,
                Avatar = Dto.TeamBasicInfo.Avatar
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
