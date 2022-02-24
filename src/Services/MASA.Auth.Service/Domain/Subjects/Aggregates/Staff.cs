namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class Staff : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public string JobNumber { get; private set; }

    public StaffStates StaffState { get; private set; }

    public StaffTypes StaffType { get; private set; }

    private Staff()
    {
        JobNumber = "";
    }

    public Staff(Guid userId, string jobNumber, StaffStates staffState, StaffTypes staffType)
    {
        UserId = userId;
        JobNumber = jobNumber;
        StaffState = staffState;
        StaffType = staffType;
    }
}
