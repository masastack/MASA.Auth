// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class TeamCommandHandler
{

    readonly ITeamRepository _teamRepository;

    readonly TeamDomainService _teamDomainService;
    readonly RoleDomainService _roleDomainService;
    readonly IClient _aliyunClient;

    string _bucket = "";
    string _cdnEndpoint = "";

    public TeamCommandHandler(ITeamRepository teamRepository,
                              TeamDomainService teamDomainService,
                              RoleDomainService roleDomainService,
                              IMasaConfiguration masaConfiguration,
                              IClient aliyunClient,
                              DaprClient daprClient)
    {
        _teamRepository = teamRepository;
        _teamDomainService = teamDomainService;
        _roleDomainService = roleDomainService;
        _aliyunClient = aliyunClient;

        _bucket = daprClient.GetSecretAsync("localsecretstore", "aliyun-oss").Result["bucket"];
        _cdnEndpoint = masaConfiguration.Local.GetValue<string>("CdnEndpoint");
    }


    [EventHandler]
    public async Task AddTeamAsync(AddTeamCommand addTeamCommand)
    {
        var dto = addTeamCommand.AddTeamDto;

        if (_teamRepository.Any(t => t.Name == dto.Name))
        {
            throw new UserFriendlyException($"Team name {dto.Name} already exists");
        }

        var teamId = Guid.NewGuid();
        var avatarName = $"{teamId}.png";

        var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.White, Color.Parse(dto.Avatar.Color), 200);
        await _aliyunClient.PutObjectAsync(_bucket, avatarName, image);

        var team = new Team(teamId, dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, $"{_cdnEndpoint}{avatarName}"));
        await _teamRepository.AddAsync(team);
        await _teamRepository.UnitOfWork.SaveChangesAsync();

        await _teamDomainService.SetTeamAdminAsync(team, dto.AdminStaffs, dto.AdminRoles, dto.AdminPermissions);
        await _teamDomainService.SetTeamMemberAsync(team, dto.MemberStaffs, dto.MemberRoles, dto.MemberPermissions);
        await _roleDomainService.UpdateRoleLimitAsync(dto.AdminRoles.Union(dto.MemberRoles));
    }

    [EventHandler]
    public async Task UpdateTeamBasicInfoAsync(UpdateTeamBasicInfoCommand updateTeamBasicInfoCommand)
    {
        var dto = updateTeamBasicInfoCommand.UpdateTeamBasicInfoDto;
        var team = await _teamRepository.GetByIdAsync(dto.Id);
        var avatarName = $"{team.Id}.png";
        if (team.Avatar.Name != dto.Avatar.Name || team.Avatar.Color != dto.Avatar.Color ||
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
        var roles = new List<Guid>();
        if (updateTeamPersonnelCommand.MemberType == TeamMemberTypes.Admin)
        {
            roles = dto.Roles.Union(team.TeamRoles.Where(tr => tr.TeamMemberType == TeamMemberTypes.Admin)
                                                    .Select(tr => tr.RoleId)).ToList();
            await _teamDomainService.SetTeamAdminAsync(team, dto.Staffs, dto.Roles, dto.Permissions);            
        }
        else
        {
            roles = dto.Roles.Union(team.TeamRoles.Where(tr => tr.TeamMemberType == TeamMemberTypes.Member)
                                        .Select(tr => tr.RoleId)).ToList();
            await _teamDomainService.SetTeamMemberAsync(team, dto.Staffs, dto.Roles, dto.Permissions);
        }
        await _roleDomainService.UpdateRoleLimitAsync(dto.Roles);
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

        await _roleDomainService.UpdateRoleLimitAsync(team.TeamRoles.Select(tr => tr.RoleId));
    }
}
