namespace Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

public class DepartmentStaff : Entity<Guid>
{
    private Department? _department;

    public Department Department
    {
        get => _department ?? throw new UserFriendlyException("Failed to get department data");
        private set => _department = value;
    }

    public Guid StaffId { get; private set; }

    public DepartmentStaff(Guid staffId)
    {
        StaffId = staffId;
    }
}
