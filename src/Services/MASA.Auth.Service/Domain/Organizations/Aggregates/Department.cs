namespace MASA.Auth.Service.Domain.Organization.Aggregates;

    public class Department : AuditAggregateRoot<Guid, Guid>
    {
    public string Name { get; set; }

    public Guid? ParentId { get; set; }

    public bool Enabled { get; set; }

    public int Sort { get; set; }

    public string Description { get; set; } = "";

    public Department(string name, string description) : this(name, description, true)
        {

        }

    public Department(string name, string description, bool enabled)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
    }
}

