namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly ITeamRepository _teamRepository;
    readonly StaffDomainService _staffDomainService;
    readonly TeamDomainService _teamDomainService;
    readonly ILdapFactory _ldapFactory;

    public CommandHandler(IUserRepository userRepository, IStaffRepository staffRepository,
        ITeamRepository teamRepository, TeamDomainService teamDomainService,
        StaffDomainService staffDomainService, ILdapFactory ldapFactory)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _teamDomainService = teamDomainService;
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
            await _userRepository.AddAsync(user);
            command.UserId = user.Id;
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

            user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Password, userDto.GenderType);
            await _userRepository.UpdateAsync(user);
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
        var ldapOptions = ldapConnectTestCommand.LDAPDetailDto.Adapt<LdapOptions>();
        if (ldapConnectTestCommand.LDAPDetailDto.IsLdaps)
        {
            ldapOptions.ServerPortSsl = ldapConnectTestCommand.LDAPDetailDto.ServerPort;
        }
        else
        {
            ldapOptions.ServerPort = ldapConnectTestCommand.LDAPDetailDto.ServerPort;
        }
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        if (!await ldapProvider.AuthenticateAsync(ldapOptions.RootUserDn, ldapOptions.RootUserPassword))
        {
            throw new UserFriendlyException("connect error");
        }
    }

    [EventHandler]
    public async Task LdapUpsertAsync(LdapUpsertCommand ldapUpsertCommand)
    {
        var ldapOptions = ldapUpsertCommand.LDAPDetailDto.Adapt<LdapOptions>();
        if (ldapUpsertCommand.LDAPDetailDto.IsLdaps)
        {
            ldapOptions.ServerPortSsl = ldapUpsertCommand.LDAPDetailDto.ServerPort;
        }
        else
        {
            ldapOptions.ServerPort = ldapUpsertCommand.LDAPDetailDto.ServerPort;
        }
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
