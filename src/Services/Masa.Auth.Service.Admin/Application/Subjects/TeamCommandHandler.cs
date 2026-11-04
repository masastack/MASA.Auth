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
    readonly IMasaConfiguration _masaConfiguration;

    string? _bucket;
    string? _cdnEndpoint;

    public TeamCommandHandler(
        ITeamRepository teamRepository,
        TeamDomainService teamDomainService,
        RoleDomainService roleDomainService,
        IMasaConfiguration masaConfiguration,
        IObjectStorageClient aliyunClient,
        IUnitOfWork unitOfWork,
        ILogger<TeamCommandHandler> logger)
    {
        _teamRepository = teamRepository;
        _teamDomainService = teamDomainService;
        _roleDomainService = roleDomainService;
        _masaConfiguration = masaConfiguration;
        _aliyunClient = aliyunClient;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// OSS 配置按需读取,不放构造函数:配置缺失时的异常应落在调用处的 try/catch 里(如 AddTeamAsync 的头像回退),
    /// 而不是让整个处理器构造失败、连带 RemoveTeam 等与 OSS 无关的命令一起不可用。
    /// </summary>
    (string Bucket, string CdnEndpoint) GetOssConfig()
    {
        if (_bucket is null || _cdnEndpoint is null)
        {
            var publicConfiguration = _masaConfiguration.ConfigurationApi.GetPublic();
            var ossOptions = publicConfiguration.GetSection(OssOptions.Key).Get<OssOptions>();
            MasaArgumentException.ThrowIfNull(ossOptions, nameof(OssOptions));

            _bucket = ossOptions.Bucket;
            _cdnEndpoint = publicConfiguration.GetValue<string>("$public.Cdn:CdnEndpoint");
        }
        return (_bucket, _cdnEndpoint!);
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
        var colorGroup = ColorPairHelper.GetColorGroup(dto.Avatar.Color);
        Team team;
        try
        {
            var (bucket, cdnEndpoint) = GetOssConfig();
            var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.ParseHex(colorGroup.FrontColor), Color.ParseHex(colorGroup.BackColor), 200);
            await _aliyunClient.PutObjectAsync(bucket, avatarName, image);
            team = new Team(teamId, dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, $"{cdnEndpoint}{avatarName}"));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            team = new Team(teamId, dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, MasaStackConsts.TEAM_AVATAR_URL));
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
        var avatarUrl = team.Avatar.Url;
        if (team.Avatar.Name != dto.Avatar.Name || team.Avatar.Color != dto.Avatar.Color ||
                string.IsNullOrWhiteSpace(team.Avatar.Url))
        {
            var (bucket, cdnEndpoint) = GetOssConfig();
            var colorGroup = ColorPairHelper.GetColorGroup(dto.Avatar.Color);
            var image = ImageSharper.GeneratePortrait(dto.Avatar.Name.FirstOrDefault(), Color.ParseHex(colorGroup.FrontColor), Color.ParseHex(colorGroup.BackColor), 200);
            await _aliyunClient.PutObjectAsync(bucket, avatarName, image);
            avatarUrl = $"{cdnEndpoint}{avatarName}";
        }
        team.UpdateBasicInfo(dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color, avatarUrl));
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
