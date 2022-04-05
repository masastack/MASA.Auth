namespace Masa.Auth.Service.Admin.Application.Subjects;

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

    #region User

    [EventHandler]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.FindAsync(u => u.PhoneNumber == userDto.PhoneNumber || u.Account == userDto.Account || u.Email == userDto.Email);
        if (user is not null)
        {
            if (userDto.PhoneNumber == user.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (userDto.Account == user.Account)
                throw new UserFriendlyException($"User with account {userDto.Account} already exists");
            if (userDto.Email == user.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
        }
        else
        {
            user = new User(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.PhoneNumber, userDto.Email, userDto.Enabled, userDto.Department, userDto.Position, userDto.Address);
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
            var existPhoneNumber = await _userRepository.GetCountAsync(u => u.Id != userDto.Id && u.PhoneNumber == userDto.PhoneNumber) > 0;
            if (existPhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");

            var existEmail = await _userRepository.GetCountAsync(u => u.Id != userDto.Id && u.Email == userDto.Email) > 0;
            if (existEmail)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");

            user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Password);
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
}
