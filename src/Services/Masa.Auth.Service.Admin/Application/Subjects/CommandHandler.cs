// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly ITeamRepository _teamRepository;
    readonly StaffDomainService _staffDomainService;
    readonly TeamDomainService _teamDomainService;
    readonly ILdapFactory _ldapFactory;
    readonly UserDomainService _userDomainService;

    public CommandHandler(IUserRepository userRepository, IStaffRepository staffRepository,
        ITeamRepository teamRepository, TeamDomainService teamDomainService, UserDomainService userDomainService,
        StaffDomainService staffDomainService, ILdapFactory ldapFactory)
    {
        _userRepository = userRepository;
        _staffRepository = staffRepository;
        _teamRepository = teamRepository;
        _staffDomainService = staffDomainService;
        _teamDomainService = teamDomainService;
        _userDomainService = userDomainService;
        _staffDomainService = staffDomainService;
        _ldapFactory = ldapFactory;
    }

    #region User

    [EventHandler]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        Expression<Func<User, bool>> condition = user => true;
        if (string.IsNullOrEmpty(userDto.PhoneNumber) is false)
            condition = condition.And(user => user.PhoneNumber == userDto.PhoneNumber);

        if (string.IsNullOrEmpty(userDto.Account) is false)
            condition = condition.And(user => user.Account == userDto.Account);

        if (string.IsNullOrEmpty(userDto.Email) is false)
            condition = condition.And(user => user.Email == userDto.Email);

        var user = await _userRepository.FindAsync(condition);
        if (user is not null)
        {
            if (string.IsNullOrEmpty(userDto.PhoneNumber) is false && userDto.PhoneNumber == user.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (string.IsNullOrEmpty(userDto.Account) is false && userDto.Account == user.Account)
                throw new UserFriendlyException($"User with account {userDto.Account} already exists");
            if (string.IsNullOrEmpty(userDto.Email) is false && userDto.Email == user.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
        }
        else
        {
            user = new User(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account ?? "", userDto.Password, userDto.CompanyName, userDto.Department, userDto.Position, userDto.Enabled, userDto.PhoneNumber ?? "", userDto.Email ?? "", userDto.Address, userDto.Gender);
            user.AddRoles(userDto.Roles.ToArray());
            user.AddPermissions(userDto.Permissions.Select(p => new UserPermission(p.PermissionId, p.Effect)).ToList());
            await _userRepository.AddAsync(user);
            command.UserId = user.Id;
            await _userDomainService.SetAsync(user);
        }
    }

    [EventHandler]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.FindAsync(u => u.Id == userDto.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");
        else
        {
            if (string.IsNullOrEmpty(userDto.PhoneNumber) is false)
            {
                var existPhoneNumber = await _userRepository.GetCountAsync(u => u.Id != userDto.Id && u.PhoneNumber == userDto.PhoneNumber) > 0;
                if (existPhoneNumber)
                    throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            }

            if (string.IsNullOrEmpty(userDto.Email) is false)
            {
                var existEmail = await _userRepository.GetCountAsync(u => u.Id != userDto.Id && u.Email == userDto.Email) > 0;
                if (existEmail)
                    throw new UserFriendlyException($"User with email {userDto.Email} already exists");
            }

            user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Password, userDto.Gender);
            await _userRepository.UpdateAsync(user);
            await _userDomainService.SetAsync(user);
        }
    }

    [EventHandler]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        var user = await _userRepository.FindAsync(u => u.Id == command.User.Id);
        if (user == null)
            throw new UserFriendlyException("The current user does not exist");

        //todo
        //Delete ThirdPartyUser
        //Delete Staff
        //Delete ...
        await _userRepository.RemoveAsync(user);
        await _userDomainService.RemoveAsync(user.Id);
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

    #region Team

    [EventHandler]
    public async Task AddTeamAsync(AddTeamCommand addTeamCommand)
    {
        var dto = addTeamCommand.AddTeamDto;
        Team team = new Team(dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color));
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
        team.UpdateBasicInfo(dto.Name, dto.Description, dto.Type, new AvatarValue(dto.Avatar.Name, dto.Avatar.Color));
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
        var ldapOptions = ldapUpsertCommand.LdapDetailDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        var ldapUsers = ldapProvider.GetAllUserAsync();
        await foreach (var ldapUser in ldapUsers)
        {
            //Public domain event
            //todo wait user memory cache
        }
    }

    #endregion
}
