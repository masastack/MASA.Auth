namespace MASA.Auth.Service.Domain.Organization.Aggregates
{
    public class DepartmentStaff : Entity<Guid>
    {
        public Guid StaffId { get; set; }

        public Guid DepartmentId { get; set; }
    }
}