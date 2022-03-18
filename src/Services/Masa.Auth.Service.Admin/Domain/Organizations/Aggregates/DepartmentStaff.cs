namespace Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

public class DepartmentStaff : AuditEntity<Guid, Guid>
{
    public Guid DepartmentId { get; private set; }

    public Guid StaffId { get; private set; }

    public Staff Staff { get; private set; } = null!;

    public DepartmentStaff(Guid staffId)
    {
        StaffId = staffId;
    }
}
