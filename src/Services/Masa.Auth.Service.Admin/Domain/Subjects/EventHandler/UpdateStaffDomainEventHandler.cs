namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class UpdateStaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IEventBus _eventBus;

    public UpdateStaffDomainEventHandler(IStaffRepository staffRepository, IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateStaffDomainEvent staffEvent)
    {
        var command = new UpdateUserCommand(staffEvent.Staff.User);
        await _eventBus.PublishAsync(command);
    }

    [EventHandler(2)]
    public async Task AddPositionAsync(UpdateStaffDomainEvent staffEvent)
    {
        var position = staffEvent.Staff.Position;
        if (position.Id == Guid.Empty)
        {
            var command = new AddPositionCommand(new AddPositionDto(position.Name));
            await _eventBus.PublishAsync(command);
            position.Id = command.PositionId;
        }
    }

    [EventHandler(3)]
    public async Task UpdateStaffAsync(UpdateStaffDomainEvent staffEvent)
    {
        var staffDto = staffEvent.Staff;
        var staff = await _staffRepository.FindAsync(s => s.JobNumber == staffDto.JobNumber);
        if (staff is not null)
            throw new UserFriendlyException($"Staff with jobNumber number {staffDto.JobNumber} already exists");

        staff = new Staff(staffDto.User.Id, staffDto.JobNumber, staffDto.User.Name, staffDto.Position.Id, staffDto.StaffType, staffDto.Enabled);
        staff.AddDepartmentStaff(staffDto.DepartmentId);
        staff.AddTeamStaff(staffDto.Teams);
        await _staffRepository.AddAsync(staff);
    }
}
