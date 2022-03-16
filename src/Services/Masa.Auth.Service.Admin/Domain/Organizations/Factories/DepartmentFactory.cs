using Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

namespace Masa.Auth.Service.Admin.Domain.Organizations.Factories;

public static class DepartmentFactory
{
    public static Department Create(string name, string description, Guid parentId, bool enabled, Guid[] staffIds)
    {
        var department = new Department(name, description);
        department.SetEnabled(enabled);
        department.Move(parentId);
        department.AddStaffs(staffIds);
        return department;
    }
}
