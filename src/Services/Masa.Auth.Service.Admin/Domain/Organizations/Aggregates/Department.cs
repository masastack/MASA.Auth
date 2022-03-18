namespace Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

public class Department : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; }

    public Guid ParentId { get; private set; }

    public bool Enabled { get; private set; } = true;

    public int Level { get; private set; } = 1;

    public int Sort { get; private set; }

    public string Description { get; private set; } = string.Empty;

    private List<DepartmentStaff> _departmentStaffs = new();

    public IReadOnlyCollection<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public Department(string name, string description) : this(name, description, null, true, new Guid[] { })
    {
    }

    public Department(string name, string description, Department? parent, bool enabled, Guid[] staffIds)
    {
        Name = name;
        Description = description;
        SetEnabled(enabled);
        AddStaffs(staffIds);
        if (parent != null)
        {
            Move(parent);
        }
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

    public void Move(Department parent)
    {
        ParentId = parent.Id;
        Level = parent.Level++;
    }

    public void DeleteCheck()
    {
        if (_departmentStaffs.Any())
        {
            throw new UserFriendlyException("The current department has staff,delete failed");
        }
    }
}

