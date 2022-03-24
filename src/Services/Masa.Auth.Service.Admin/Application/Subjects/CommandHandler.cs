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
            user = new User(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Email, userDto.Department, userDto.Position, userDto.Address);           
            await _userRepository.AddAsync(user);
        }
    }

    [EventHandler]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.FindAsync(u => u.Id != userDto.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");
        else
        {
            if (userDto.PhoneNumber == user.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (userDto.Email == user.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
            user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Password);
            await _userRepository.UpdateAsync(user);
        }
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
        //Delete ...
        await _userRepository.RemoveAsync(user);
    }

    [EventHandler]
    public async Task CreateStaffAsync(AddStaffCommand createStaffCommand)
    {
        //_staffDomainService.CreateStaff();
        var staff = new Staff(createStaffCommand.JobNumber, createStaffCommand.CreateUserCommand.User.Name,
            createStaffCommand.StaffType, createStaffCommand.Enabled);
        var users = await _userRepository.GetListAsync(u => u.PhoneNumber == createStaffCommand.CreateUserCommand.User.PhoneNumber);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            var userDto = createStaffCommand.CreateUserCommand.User;
            user = new User(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard,
                userDto.Account, userDto.Password, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Email, "", userDto.Position);
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
