using Masa.Auth.Service.Domain.Subjects.Services;

namespace Masa.Auth.Service.Application.Subjects;

public class CommandHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IUserRepository _userRepository;
    readonly StaffDomainService _staffDomainService;

    public CommandHandler(IStaffRepository staffRepository, IUserRepository userRepository, StaffDomainService staffDomainService)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _staffDomainService = staffDomainService;
    }

    [EventHandler]
    public async Task CreateUserAsync(CreateUserCommand createUserCommand)
    {

    }

    [EventHandler]
    public async Task CreateStaffAsync(CreateStaffCommand createStaffCommand)
    {
        //_staffDomainService.CreateStaff();
        var staff = new Staff(createStaffCommand.JobNumber, createStaffCommand.CreateUserCommand.Name,
            createStaffCommand.StaffType, createStaffCommand.Enabled);
        var users = await _userRepository.GetListAsync(u => u.PhoneNumber == createStaffCommand.CreateUserCommand.PhoneNumber);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            var userInfo = createStaffCommand.CreateUserCommand;
            user = new User(userInfo.Name, userInfo.DisplayName, userInfo.Avatar, userInfo.IDCard,
                userInfo.Account, userInfo.Password, userInfo.CompanyName, userInfo.Enabled, userInfo.PhoneNumber, userInfo.Email);
        }
        else
        {
            //update user info

        }
        staff.BindUser(user);
        await _staffRepository.AddAsync(staff);
    }

    [EventHandler]
    public async Task DeleteStaffAsync(DeleteStaffCommand deleteStaffCommand)
    {
        var staff = await _staffRepository.FindAsync(deleteStaffCommand.StaffId);
        if (staff == null)
        {
            throw new UserFriendlyException("the id of staff not found");
        }
        await _staffRepository.RemoveAsync(staff);
    }
}
