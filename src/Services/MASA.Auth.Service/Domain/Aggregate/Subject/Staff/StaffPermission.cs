namespace MASA.Auth.Service.Domain.Aggregate
{
    public class StaffPermission : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid? PermissionId { get; set; }
    }
}