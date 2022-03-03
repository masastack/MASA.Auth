namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class StaffRole : Entity<Guid>
{
    public Guid StaffId { get; set; }

    public Guid? RoleId { get; set; }
}
