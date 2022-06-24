// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ITeamRepository _teamRepository;
    readonly StaffDomainService _staffDomainService;
    readonly TeamDomainService _teamDomainService;
    readonly ILdapFactory _ldapFactory;
    readonly UserDomainService _userDomainService;
    readonly ThirdPartyUserDomainService _thirdPartyUserDomainService;
    readonly IConfiguration _configuration;
    readonly ILogger<CommandHandler> _logger;
    readonly IClient _aliyunClient;

    string _bucket = "";
    string _cdnEndpoint = "";

    public CommandHandler(IUserRepository userRepository, IStaffRepository staffRepository, IThirdPartyIdpRepository thirdPartyIdpRepository,
        ITeamRepository teamRepository, StaffDomainService staffDomainService, TeamDomainService teamDomainService, ILdapFactory ldapFactory,
        UserDomainService userDomainService, ThirdPartyUserDomainService thirdPartyUserDomainService, ILdapIdpRepository ldapIdpRepository,
        IConfiguration configuration, ILogger<CommandHandler> logger, IClient aliyunClient, DaprClient daprClient)
    {
        _userRepository = userRepository;
        _staffRepository = staffRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _teamRepository = teamRepository;
        _staffDomainService = staffDomainService;
        _teamDomainService = teamDomainService;
        _ldapFactory = ldapFactory;
        _userDomainService = userDomainService;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
        _ldapIdpRepository = ldapIdpRepository;
        _configuration = configuration;
        _logger = logger;
        _aliyunClient = aliyunClient;

        _bucket = daprClient.GetSecretAsync("localsecretstore", "bucket").Result.FirstOrDefault().Value;
        _cdnEndpoint = _configuration.GetValue<string>("CdnEndpoint");
    }

    #region User

    [EventHandler(1)]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        Expression<Func<User, bool>> condition = user => user.Account == userDto.Account;
        if (!string.IsNullOrEmpty(userDto.PhoneNumber))
            condition = condition.Or(user => user.PhoneNumber == userDto.PhoneNumber);
        if (!string.IsNullOrEmpty(userDto.Landline))
            condition = condition.Or(user => user.Landline == userDto.Landline);
        if (!string.IsNullOrEmpty(userDto.Email))
            condition = condition.Or(user => user.Email == userDto.Email);
        if (!string.IsNullOrEmpty(userDto.IdCard))
            condition = condition.Or(user => user.IdCard == userDto.IdCard);

        var user = await _userRepository.FindAsync(condition);
        if (user is not null)
        {        
            if (string.IsNullOrEmpty(userDto.PhoneNumber) is false && userDto.PhoneNumber == user.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (string.IsNullOrEmpty(userDto.Landline) is false && userDto.Landline == user.Landline)
                throw new UserFriendlyException($"User with landline {userDto.Landline} already exists");
            if (string.IsNullOrEmpty(userDto.Account) is false && userDto.Account == user.Account)
                throw new UserFriendlyException($"User with account {userDto.Account} already exists");
            if (string.IsNullOrEmpty(userDto.Email) is false && userDto.Email == user.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
            if (string.IsNullOrEmpty(userDto.IdCard) is false && userDto.IdCard == user.IdCard)
                throw new UserFriendlyException($"User with idCard {userDto.IdCard} already exists");
        }
        else
        {
            user = new User(userDto.Name, userDto.DisplayName ?? "", userDto.Avatar ?? "", userDto.IdCard ?? "", userDto.Account ?? "", userDto.Password, userDto.CompanyName ?? "", userDto.Department ?? "", userDto.Position ?? "", userDto.Enabled, userDto.PhoneNumber ?? "", userDto.Landline, userDto.Email ?? "", userDto.Address, userDto.Gender);
            user.AddRoles(userDto.Roles.ToArray());
            user.AddPermissions(userDto.Permissions.Select(p => new UserPermission(p.PermissionId, p.Effect)).ToList());
            await _userRepository.AddAsync(user);
            command.NewUser = user;
            await _userDomainService.SetAsync(user);
        }
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.FindAsync(u => u.Id == userDto.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        Expression<Func<User, bool>> condition = user => false;
        if (!string.IsNullOrEmpty(userDto.PhoneNumber))
            condition = condition.Or(user => user.PhoneNumber == userDto.PhoneNumber);
        if (!string.IsNullOrEmpty(userDto.Landline))
            condition = condition.Or(user => user.Landline == userDto.Landline);
        if (!string.IsNullOrEmpty(userDto.Email))
            condition = condition.Or(user => user.Email == userDto.Email);
        if (!string.IsNullOrEmpty(userDto.IdCard))
            condition = condition.Or(user => user.IdCard == userDto.IdCard);

        Expression<Func<User, bool>> condition2 = user => user.Id != userDto.Id;
        var exitUser = await _userRepository.FindAsync(condition2.And(condition));
        if (exitUser is not null)
        {
            if (string.IsNullOrEmpty(userDto.PhoneNumber) is false && userDto.PhoneNumber == exitUser.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (string.IsNullOrEmpty(userDto.Landline) is false && userDto.Landline == exitUser.Landline)
                throw new UserFriendlyException($"User with landline {userDto.Landline} already exists");           
            if (string.IsNullOrEmpty(userDto.Email) is false && userDto.Email == exitUser.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
            if (string.IsNullOrEmpty(userDto.IdCard) is false && userDto.IdCard == exitUser.IdCard)
                throw new UserFriendlyException($"User with idCard {userDto.IdCard} already exists");
        }
        else
        {
            user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
            await _userRepository.UpdateAsync(user);
            await _userDomainService.SetAsync(user);
        }
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        var user = await _userRepository.FindAsync(u => u.Id == command.User.Id);
        if (user == null)
            throw new UserFriendlyException("The current user does not exist");

        await _userRepository.RemoveAsync(user);
        await _userDomainService.RemoveAsync(user.Id);
    }

    [EventHandler(1)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.GetDetailAsync(userDto.Id);
        if (user == null)
            throw new UserFriendlyException("The current user does not exist");

        user.AddRoles(userDto.Roles.ToArray());
        user.AddPermissions(userDto.Permissions.Select(p => new UserPermission(p.PermissionId, p.Effect)).ToList());
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task UserValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        var user = await _userRepository.FindAsync(u => u.Account == validateByAccountCommand.Account && u.Password == validateByAccountCommand.Password);
        validateByAccountCommand.Result = user != null;
    }
    #endregion

    #region Staff

    [EventHandler]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await _staffDomainService.AddStaffAsync(command.Staff);
    }

    [EventHandler]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        await _staffDomainService.UpdateStaffAsync(command.Staff);
    }

    [EventHandler]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        var staff = await _staffRepository.FindAsync(command.Staff.Id);
        if (staff == null)
        {
            throw new UserFriendlyException("the current staff not found");
        }
        await _staffRepository.RemoveAsync(staff);
    }

    [EventHandler]
    public async Task SyncAsync(SyncStaffCommand command)
    {
        command.Result = await _staffDomainService.SyncStaffAsync(command.Staffs);
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler]
    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var exist = await _thirdPartyIdpRepository.GetCountAsync(thirdPartyIdp => thirdPartyIdp.Name == thirdPartyIdpDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ThirdPartyIdp with name {thirdPartyIdpDto.Name} already exists");

        var thirdPartyIdp = new ThirdPartyIdp(thirdPartyIdpDto.Name, thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, default, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);

        await _thirdPartyIdpRepository.AddAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == thirdPartyIdpDto.Id);
        if (thirdPartyIdp is null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        thirdPartyIdp.Update(thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, default, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);
        await _thirdPartyIdpRepository.UpdateAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task RemoveThirdPartyIdpAsync(RemoveThirdPartyIdpCommand command)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == command.ThirdPartyIdp.Id);
        if (thirdPartyIdp == null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        await _thirdPartyIdpRepository.RemoveAsync(thirdPartyIdp);
    }

    #endregion

    #region Team

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

    #endregion

    #region Ldap

    [EventHandler]
    public async Task LdapConnectTestAsync(LdapConnectTestCommand ldapConnectTestCommand)
    {
        var ldapOptions = ldapConnectTestCommand.LdapDetailDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        if (!await ldapProvider.AuthenticateAsync(ldapOptions.RootUserDn, ldapOptions.RootUserPassword))
        {
            throw new UserFriendlyException("connect error");
        }
    }

    [EventHandler]
    public async Task LdapUpsertAsync(LdapUpsertCommand ldapUpsertCommand)
    {
        var _thirdPartyIdpId = Guid.Empty;
        var ldapIdp = new LdapIdp(ldapUpsertCommand.LdapDetailDto.ServerAddress, ldapUpsertCommand.LdapDetailDto.ServerPort, ldapUpsertCommand.LdapDetailDto.IsLdaps,
                ldapUpsertCommand.LdapDetailDto.BaseDn, ldapUpsertCommand.LdapDetailDto.RootUserDn, ldapUpsertCommand.LdapDetailDto.RootUserPassword, ldapUpsertCommand.LdapDetailDto.UserSearchBaseDn, ldapUpsertCommand.LdapDetailDto.GroupSearchBaseDn);
        var dbItem = await _ldapIdpRepository.FindAsync(l => l.Name == ldapIdp.Name);
        if (dbItem is null)
        {
            await _ldapIdpRepository.AddAsync(ldapIdp);
            await _ldapIdpRepository.UnitOfWork.SaveChangesAsync();
            _thirdPartyIdpId = ldapIdp.Id;
        }
        else
        {
            _thirdPartyIdpId = dbItem.Id;
            dbItem.Update(ldapIdp);
            await _ldapIdpRepository.UpdateAsync(dbItem);
        }

        var ldapOptions = ldapUpsertCommand.LdapDetailDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        var ldapUsers = ldapProvider.GetAllUserAsync();
        await foreach (var ldapUser in ldapUsers)
        {
            try
            {
                //todo:change bulk
                var thirdPartyUserDtp = new AddThirdPartyUserDto(_thirdPartyIdpId, true, ldapUser.ObjectGuid,
                    new AddUserDto
                    {
                        Name = ldapUser.Name,
                        DisplayName = ldapUser.DisplayName,
                        Enabled = true,
                        Email = ldapUser.EmailAddress,
                        Account = ldapUser.SamAccountName,
                        Password = _configuration.GetValue<string>("Subjects:InitialPassword"),
                        Avatar = _configuration.GetValue<string>("Subjects:Avatar")
                    });
                //phone number regular match
                if (Regex.IsMatch(ldapUser.Phone, @"^1[3456789]\d{9}$"))
                {
                    thirdPartyUserDtp.User.PhoneNumber = ldapUser.Phone;
                }
                else
                {
                    thirdPartyUserDtp.User.Landline = ldapUser.Phone;
                }
                await _thirdPartyUserDomainService.AddThirdPartyUserAsync(thirdPartyUserDtp);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "LdapUser Name = {0},Email = {1},PhoneNumber={2}", ldapUser.Name, ldapUser.EmailAddress, ldapUser.Phone);
            }
        }
    }
    #endregion
}
