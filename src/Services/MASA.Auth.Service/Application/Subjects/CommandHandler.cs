using MASA.Auth.Service.Application.Subjects.Commands;

namespace MASA.Auth.Service.Application.Subjects;

public class CommandHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IStaffRepository _userRepository;
    readonly EventBus _eventBus;

    public CommandHandler(IStaffRepository staffRepository, IStaffRepository userRepository, EventBus eventBus)
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
        var user = await _userRepository.FindAsync(createStaffCommand.UserId);
        if (user != null)
        {
            await _eventBus.PublishAsync(new UpdateUserCommand());
        }
        //
        //await _eventBus.PublishAsync(new CreateUserCommand());
    }
}
