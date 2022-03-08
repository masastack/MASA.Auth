namespace MASA.Auth.Service.Domain.Organization.Aggregates;

public class Department : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; set; }

    public Guid ParentId { get; set; } = Guid.Empty;

    public bool Enabled { get; set; } = true;

    public int Sort { get; set; }

    public string Description { get; set; } = "";

    private List<DepartmentStaff> _departmentStaffs = new();

    public IReadOnlyCollection<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public Department(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void AddStaffs(params Guid[] staffIds)
    {
        foreach (var staffId in staffIds)
        {
            _departmentStaffs.Add(new DepartmentStaff(staffId));
        }
    }

    public void RemoveStaffs(params Guid[] staffIds)
    {
        _departmentStaffs.RemoveAll(ds => staffIds.Contains(ds.StaffId));
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }

    public void Move(Guid parentId)
    {
        ParentId = parentId;
    }
}

