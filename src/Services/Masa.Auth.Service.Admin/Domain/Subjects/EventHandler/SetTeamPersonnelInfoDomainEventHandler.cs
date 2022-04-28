namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class SetTeamPersonnelInfoDomainEventHandler
{
    readonly ITeamRepository _teamRepository;
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;

    public SetTeamPersonnelInfoDomainEventHandler(ITeamRepository teamRepository, IUserRepository userRepository, IStaffRepository staffRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _staffRepository = staffRepository;
    }

    [EventHandler(2)]
    public async Task SetUserAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        var oldStaffIds = setTeamPersonnelInfoDomainEvent.Team.TeamStaffs.Select(ts => ts.StaffId);
        //get remove staff 
        var removeStaffIds = oldStaffIds.Except(setTeamPersonnelInfoDomainEvent.StaffIds);
        //get add staff 
        var addStaffIds = setTeamPersonnelInfoDomainEvent.StaffIds.Except(oldStaffIds);
        //update team role id
        var removeUsers = (await _staffRepository.GetListAsync(s => removeStaffIds.Contains(s.Id))).Select(s => s.User).ToList();
        var addUsers = (await _staffRepository.GetListAsync(s => removeStaffIds.Contains(s.Id))).Select(s => s.User).ToList();
        var teamRoleId = setTeamPersonnelInfoDomainEvent.Type == TeamMemberTypes.Admin ?
                        setTeamPersonnelInfoDomainEvent.Team.GetAdminRoleId() :
                        setTeamPersonnelInfoDomainEvent.Team.GetMemberRoleId();
        removeUsers.ForEach(user =>
        {
            user.RemoveRoles(teamRoleId);
        });
        addUsers.ForEach(user =>
        {
            user.AddRoles(teamRoleId);
        });
        await _userRepository.UpdateRangeAsync(removeUsers);
        await _userRepository.UpdateRangeAsync(addUsers);
        await _userRepository.UnitOfWork.SaveChangesAsync();
        setTeamPersonnelInfoDomainEvent.Team.SetStaff(setTeamPersonnelInfoDomainEvent.Type, setTeamPersonnelInfoDomainEvent.StaffIds);
    }

    [EventHandler(4)]
    public async Task SaveTeamAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        await _teamRepository.UpdateAsync(setTeamPersonnelInfoDomainEvent.Team);
    }
}
