﻿namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly StaffDomainService _staffDomainService;

    public CommandHandler(IUserRepository userRepository, IStaffRepository staffRepository, StaffDomainService staffDomainService)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _staffDomainService = staffDomainService;
    }

    [EventHandler]
    public async Task AddUserAsync(AddUserCommand command)
    {
        if (await _userRepository.GetCountAsync(u => u.PhoneNumber == command.PhoneNumber) > 0)
            throw new UserFriendlyException($"User with phone number {command.PhoneNumber} already exists");

        var user = new User(command.Name, command.DisplayName, command.Avatar, command.IdCard, command.PhoneNumber, "", command.CompanyName, command.Enabled, command.PhoneNumber, command.Email, command.householdAddress, command.residentialAddress);
        await _userRepository.AddAsync(user);
    }

    [EventHandler]
    public async Task EditUserAsync(UpdateUserCommand command)
    {
        var user = await _userRepository.FindAsync(u => u.Id == command.UserId);
        if (user is null)
            throw new UserFriendlyException($"The current user does not exist");

        user.Update();
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task DeleteUserAsync(RemoveUserCommand command)
    {
        var user = await _userRepository.FindAsync(u => u.Id == command.UserId);
        if (user == null)
            throw new UserFriendlyException("The current user does not exist");

        //todo
        //Delete ThirdPartyUser
        //Delete Staff
        await _userRepository.RemoveAsync(user);
    }

    [EventHandler]
    public async Task CreateStaffAsync(AddStaffCommand createStaffCommand)
    {
        //_staffDomainService.CreateStaff();
        var staff = new Staff(createStaffCommand.JobNumber, createStaffCommand.CreateUserCommand.Name,
            createStaffCommand.StaffType, createStaffCommand.Enabled);
        var users = await _userRepository.GetListAsync(u => u.PhoneNumber == createStaffCommand.CreateUserCommand.PhoneNumber);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            var userInfo = createStaffCommand.CreateUserCommand;
            user = new User(userInfo.Name, userInfo.DisplayName, userInfo.Avatar, userInfo.IdCard,
                userInfo.Account, userInfo.Password, userInfo.CompanyName, userInfo.Enabled, userInfo.PhoneNumber, userInfo.Email);
        }
        else
        {
            //todo update user info

        }
        staff.BindUser(user);
        await _staffRepository.AddAsync(staff);
    }

    [EventHandler]
    public async Task DeleteStaffAsync(RemoveStaffCommand deleteStaffCommand)
    {
        var staff = await _staffRepository.FindAsync(deleteStaffCommand.StaffId);
        if (staff == null)
        {
            throw new UserFriendlyException("the id of staff not found");
        }
        await _staffRepository.RemoveAsync(staff);
    }
}
