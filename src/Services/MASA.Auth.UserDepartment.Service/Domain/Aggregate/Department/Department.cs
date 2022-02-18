namespace MASA.Auth.UserDepartment.Domain.Aggregate
{
    public class Department : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }

        public int Sort { get; set; }

        public Guid? ParentId { get; set; }

        public string? Describe { get; set; }

        public Department(string name, int sort) => (Name, Sort) = (name, sort);
    }
}
