namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class AddStaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IEventBus _eventBus;

    public AddStaffDomainEventHandler(IStaffRepository staffRepository, IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task AddUserAsync(AddStaffDomainEvent staffEvent)
    {
        var command = new AddUserCommand(staffEvent.Staff.User);
        await _eventBus.PublishAsync(command);
        staffEvent.Staff.UserId = command.UserId;
    }

    [EventHandler(2)]
    public async Task AddOrUpdatePositionAsync(AddStaffDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Staff.Position.Name) is false)
        {
            var command = new AddOrUpdatePositionCommand(staffEvent.Staff.Position);
            await _eventBus.PublishAsync(command);
        }
    }

    [EventHandler(3)]
    public async Task AddStaffAsync(AddStaffDomainEvent staffEvent)
    {
        var staffDto = staffEvent.Staff;
        var staff = await _staffRepository.FindAsync(s => s.JobNumber == staffDto.JobNumber);
        if (staff is not null)
            throw new UserFriendlyException($"Staff with jobNumber number {staffDto.JobNumber} already exists");

        staff = new Staff(staffDto.UserId, staffDto.JobNumber, staffDto.User.Name, staffDto.Position.Id, staffDto.StaffType, staffDto.Enabled);
        staff.AddDepartmentStaff(staffDto.DepartmentId);
        staff.AddTeamStaff(staffDto.Teams);
        await _staffRepository.AddAsync(staff);
    }
}
