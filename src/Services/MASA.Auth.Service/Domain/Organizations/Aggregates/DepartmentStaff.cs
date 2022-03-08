namespace MASA.Auth.Service.Domain.Organizations.Aggregates;

public class DepartmentStaff : Entity<Guid>
{
    public Guid StaffId { get; private set; }

    public DepartmentStaff(Guid staffId)
    {
        StaffId = staffId;
    }
}
