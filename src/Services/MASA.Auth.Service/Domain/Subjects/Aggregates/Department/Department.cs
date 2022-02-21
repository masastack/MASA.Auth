namespace MASA.Auth.Service.Domain.Subjects.Aggregates.Department
{
    public class Department : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }

        public int Sort { get; set; }

        public Guid? ParentId { get; set; }

        public string? Describe { get; set; }

        private Department()
        {
            Name = "";
        }

        public Department(string name, int sort) => (Name, Sort) = (name, sort);
    }
}
