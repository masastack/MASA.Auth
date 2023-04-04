// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class TeamCommandHandler
{

    readonly ITeamRepository _teamRepository;
    readonly TeamDomainService _teamDomainService;
    readonly RoleDomainService _roleDomainService;
    readonly IObjectStorageClient _aliyunClient;
    readonly IUnitOfWork _unitOfWork;
    readonly ILogger<TeamCommandHandler> _logger;

    string _bucket = "";
    string _cdnEndpoint = "";

    public TeamCommandHandler(
        ITeamRepository teamRepository,
        TeamDomainService teamDomainService,
        RoleDomainService roleDomainService,
        IMasaConfiguration masaConfiguration,
        IObjectStorageClient aliyunClient,
        IOptions<OssOptions> ossOptions,
        IUnitOfWork unitOfWork,
        ILogger<TeamCommandHandler> logger)
    {
        _teamRepository = teamRepository;
        _teamDomainService = teamDomainService;
        _roleDomainService = roleDomainService;
        _aliyunClient = aliyunClient;
        _bucket = ossOptions.Value.Bucket;
        _cdnEndpoint = masaConfiguration.ConfigurationApi.GetPublic().GetValue<string>("$public.Cdn:CdnEndpoint");
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [EventHandler(1)]
    public async Task AddTeamAsync(AddTeamCommand addTeamCommand)
    {
        var dto = addTeamCommand.AddTeamDto;

        if (_teamRepository.Any(t => t.Name == dto.Name))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.TEAM_NAME_EXIST, dto.Name);
        }

        var teamId = Guid.NewGuid();
        var avatarName = $"{teamId}.png";
        if (!ColorGroupConstants.ColorGroup.TryGetValue(dto.Avatar.Color, out var color))
        {
            color = ColorGroupConstants.DEFAULT_COLOR;
        }
        Team team;
        try
        {
            var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.White, Color.ParseHex(color), 200);
            await _aliyunClient.PutObjectAsync(_bucket, avatarName, image);
            team = new Team(teamId, dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, $"{_cdnEndpoint}{avatarName}"));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            team = new Team(teamId, dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color));
        }

        if (dto.Id != Guid.Empty)
        {
            team.SetId(dto.Id);
        }

        await _teamRepository.AddAsync(team);
        await _unitOfWork.SaveChangesAsync();

        await _teamDomainService.SetTeamAdminAsync(team, dto.AdminStaffs, dto.AdminRoles, dto.AdminPermissions);
        await _teamDomainService.SetTeamMemberAsync(team, dto.MemberStaffs, dto.MemberRoles, dto.MemberPermissions);
        await _roleDomainService.UpdateRoleLimitAsync(dto.AdminRoles.Union(dto.MemberRoles));
        await _unitOfWork.SaveChangesAsync();
        addTeamCommand.Result = team!.Id;
    }

    [EventHandler(1)]
    public async Task UpdateTeamAsync(UpdateTeamCommand updateTeamCommand)
    {
        var dto = updateTeamCommand.UpdateTeamDto;
        if (_teamRepository.Any(t => t.Name == dto.Name && t.Id != dto.Id))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.TEAM_NAME_EXIST, dto.Name);
        }
        var team = await _teamRepository.GetByIdAsync(dto.Id);
        var teamRoles = team.TeamRoles.ToArray();
        var avatarName = $"{team.Id}.png";
        if (team.Avatar.Name != dto.Avatar.Name || team.Avatar.Color != dto.Avatar.Color ||
                string.IsNullOrWhiteSpace(team.Avatar.Url))
        {
            if (!ColorGroupConstants.ColorGroup.TryGetValue(dto.Avatar.Color, out var color))
            {
                color = ColorGroupConstants.DEFAULT_COLOR;
            }
            var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.White, Color.ParseHex(color), 200);
            await _aliyunClient.PutObjectAsync(_bucket, avatarName, image);
        }
        team.UpdateBasicInfo(dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, $"{_cdnEndpoint}{avatarName}"));
        await _teamRepository.UpdateAsync(team);

        //todo Add update judgment
        await _teamDomainService.SetTeamAdminAsync(team, dto.AdminStaffs, dto.AdminRoles, dto.AdminPermissions);
        await _teamDomainService.SetTeamMemberAsync(team, dto.MemberStaffs, dto.MemberRoles, dto.MemberPermissions);

        await _roleDomainService.UpdateRoleLimitAsync(dto.AdminRoles.Union(dto.MemberRoles).Union(team.TeamRoles.Select(tr => tr.RoleId)).Union(teamRoles.Select(tr => tr.RoleId)));
    }

    [EventHandler(1)]
    public async Task RemoveTeamAsync(RemoveTeamCommand removeTeamCommand)
    {
        var team = await _teamRepository.GetByIdAsync(removeTeamCommand.TeamId);
        if (team.TeamStaffs.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.TEAM_HAS_STAFF_DELETE);
        }
        await _teamRepository.RemoveAsync(team);

        await _roleDomainService.UpdateRoleLimitAsync(team.TeamRoles.Select(tr => tr.RoleId));
        removeTeamCommand.Result = team;
    }
}
