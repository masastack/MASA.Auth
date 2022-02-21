namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class StaffPermission : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; private set; }

        public bool Effect { get; private set; }

        public Guid? PermissionId { get; private set; }

        public StaffPermission(Guid staffId, bool effect, Guid? permissionId)
        {
            StaffId = staffId;
            Effect = effect;
            PermissionId = permissionId;
        }
    }
}