namespace Masa.Auth.Service.Domain.Organizations.Aggregates;

public class DepartmentStaff : Entity<Guid>
{
    public Guid StaffId { get; private set; }

    public Department Department { get; private set; } = null!;

    public DepartmentStaff(Guid staffId)
    {
        StaffId = staffId;
    }
}
