namespace Masa.Auth.Service.Application.Subjects;

public class CommandHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IUserRepository _userRepository;
    readonly IEventBus _eventBus;

    public CommandHandler(IStaffRepository staffRepository, IUserRepository userRepository, IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task CreateUserAsync(CreateUserCommand createUserCommand)
    {

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
