namespace Masa.Auth.Service.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly IEventBus _eventBus;

    public CommandHandler(IUserRepository userRepository, IStaffRepository staffRepository, IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task AddUserAsync(AddUserCommand command)
    {
        if (await _userRepository.GetCountAsync(u => u.PhoneNumber == command.PhoneNumber) > 0)
            throw new UserFriendlyException($"User with phone number {command.PhoneNumber} already exists");

        var user = new User(command.Name, command.DisplayName, command.Avatar, command.IDCard, command.PhoneNumber,"",command.CompanyName,command.Enabled,command.PhoneNumber,command.Email, command.householdAddress, command.residentialAddress);
        await _userRepository.AddAsync(user);
    }

    [EventHandler]
    public async Task EditUserAsync(EditUserCommand command)
    {
        var user = await _userRepository.FindAsync(u => u.Id == command.UserId);
        if (user is null)
            throw new UserFriendlyException($"The current User does not exist");

        user.Update();
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task DeleteUserAsync(DeleteUserCommand command)
    {
        var user = await _userRepository.FindAsync(u => u.Id == command.UserId);
        if (user == null)
            throw new UserFriendlyException("The current role does not exist");

        //Toto
        //Delete ThirdPartyUser
        //Delete Staff
        await _userRepository.RemoveAsync(user);
    }

    [EventHandler]
    public async Task CreateStaffAsync(CreateStaffCommand createStaffCommand)
    {
        var users = await _userRepository.GetListAsync(u => u.PhoneNumber == createStaffCommand.CreateUserCommand.PhoneNumber);
        //if (user != null)
        //{
        //    await _eventBus.PublishAsync(new UpdateUserCommand());
        //}
        //
        //await _eventBus.PublishAsync(new CreateUserCommand());

    }
}
