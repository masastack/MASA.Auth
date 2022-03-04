namespace MASA.Auth.Service.Domain.Organization.Aggregates;

public class Department : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; set; }

    public Guid ParentId { get; set; } = Guid.Empty;

    public bool Enabled { get; set; }

    public int Sort { get; set; }

    public string Description { get; set; } = "";

    private List<DepartmentStaff> _departmentStaffs = new();

    public IReadOnlyCollection<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public Department(string name, string description) : this(name, description, true)
    {

    }

    public Department(string name, string description, bool enabled)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
    }

    public void AddStaffs(params Guid[] staffIds)
    {
        foreach (var staffId in staffIds)
        {
            _departmentStaffs.Add(new DepartmentStaff(staffId));
        }
    }

    public void Move(Guid parentId)
    {
        ParentId = parentId;
    }
}

