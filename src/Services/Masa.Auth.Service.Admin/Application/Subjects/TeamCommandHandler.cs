// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class TeamCommandHandler
{

    readonly ITeamRepository _teamRepository;

    readonly TeamDomainService _teamDomainService;
    readonly IClient _aliyunClient;

    string _bucket = "";
    string _cdnEndpoint = "";

    public TeamCommandHandler(ITeamRepository teamRepository,
                              TeamDomainService teamDomainService,
                              IMasaConfiguration masaConfiguration,
                              IClient aliyunClient,
                              DaprClient daprClient)
    {
        _teamRepository = teamRepository;
        _teamDomainService = teamDomainService;
        _aliyunClient = aliyunClient;

        _bucket = daprClient.GetSecretAsync("localsecretstore", "bucket").Result.FirstOrDefault().Value;
        _cdnEndpoint = masaConfiguration.GetConfiguration(SectionTypes.Local).GetValue<string>("CdnEndpoint");
    }


    [EventHandler]
    public async Task AddTeamAsync(AddTeamCommand addTeamCommand)
    {
        var dto = addTeamCommand.AddTeamDto;
        var teamId = Guid.NewGuid();
        var avatarName = $"{teamId}.png";

        var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.White, Color.Parse(dto.Avatar.Color), 200);
        await _aliyunClient.PutObjectAsync(_bucket, avatarName, image);

        Team team = new Team(teamId, dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, $"{_cdnEndpoint}{avatarName}"));
        await _teamRepository.AddAsync(team);
        await _teamRepository.UnitOfWork.SaveChangesAsync();

        await _teamDomainService.SetTeamAdminAsync(team, dto.AdminStaffs, dto.AdminRoles, dto.AdminPermissions);
        await _teamDomainService.SetTeamMemberAsync(team, dto.MemberStaffs, dto.MemberRoles, dto.MemberPermissions);
    }

    [EventHandler]
    public async Task UpdateTeamBasicInfoAsync(UpdateTeamBasicInfoCommand updateTeamBasicInfoCommand)
    {
        var dto = updateTeamBasicInfoCommand.UpdateTeamBasicInfoDto;
        var team = await _teamRepository.GetByIdAsync(dto.Id);
        var avatarName = $"{team.Id}.png";
        if ((team.Avatar.Name != dto.Avatar.Name && team.Avatar.Color != dto.Avatar.Color) ||
                string.IsNullOrWhiteSpace(team.Avatar.Url))
        {
            var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.White, Color.Parse(dto.Avatar.Color), 200);
            await _aliyunClient.PutObjectAsync(_bucket, avatarName, image);
        }
        team.UpdateBasicInfo(dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, $"{_cdnEndpoint}{avatarName}"));
        await _teamRepository.UpdateAsync(team);
    }

    [EventHandler]
    public async Task UpdateTeamAdminAsync(UpdateTeamPersonnelCommand updateTeamPersonnelCommand)
    {
        var dto = updateTeamPersonnelCommand.UpdateTeamPersonnelDto;
        var team = await _teamRepository.GetByIdAsync(dto.Id);
        if (updateTeamPersonnelCommand.MemberType == TeamMemberTypes.Admin)
        {
            await _teamDomainService.SetTeamAdminAsync(team, dto.Staffs, dto.Roles, dto.Permissions);
        }
        else
        {
            await _teamDomainService.SetTeamMemberAsync(team, dto.Staffs, dto.Roles, dto.Permissions);
        }
    }


    [EventHandler]
    public async Task RemoveTeamAsync(RemoveTeamCommand removeTeamCommand)
    {
        var team = await _teamRepository.GetByIdAsync(removeTeamCommand.TeamId);
        if (team.TeamStaffs.Any())
        {
            throw new UserFriendlyException("the team has staffs can`t delete");
        }
        await _teamRepository.RemoveAsync(team);
    }

}
