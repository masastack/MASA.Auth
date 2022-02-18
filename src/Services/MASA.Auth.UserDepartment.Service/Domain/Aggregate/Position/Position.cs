namespace MASA.Auth.UserDepartment.Domain.Aggregate
{
    public class Position : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public Position(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
