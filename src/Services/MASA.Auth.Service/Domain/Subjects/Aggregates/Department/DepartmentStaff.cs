namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class DepartmentStaff : Entity<Guid>
    {
        public Guid StaffId { get; set; }

        public Guid DepartmentId { get; set; }
    }
}