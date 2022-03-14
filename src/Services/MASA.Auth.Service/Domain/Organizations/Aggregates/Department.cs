namespace Masa.Auth.Service.Domain.Organizations.Aggregates;

public class Department : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    public Guid ParentId { get; private set; }

    public bool Enabled { get; private set; } = true;

    public int Sort { get; private set; }

    public string Description { get; private set; } = "";

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

    public void DeleteCheck()
    {
        if (_departmentStaffs.Any())
        {
            throw new UserFriendlyException("The current department has staff,delete failed");
        }
    }
}

