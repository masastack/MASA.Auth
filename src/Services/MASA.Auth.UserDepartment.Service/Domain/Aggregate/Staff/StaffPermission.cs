namespace MASA.Auth.UserDepartment.Domain.Aggregate.Staff
{
    public class StaffPermission : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid? PermissionId { get; set; }     
    }
}