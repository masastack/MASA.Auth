namespace MASA.Auth.Service.Application.Organizations.Commands;

public record CopyDepartmentCommand : CreateDepartmentCommand
{
    public bool IsMigrate { get; set; }

    public Guid OriginalId { get; set; }

    public CopyDepartmentCommand(Guid OriginalId, string Name, string Description, Guid ParentId, List<Guid> StaffIds, bool Enabled = true, bool IsMigrate = true)
        : base(Name, Description, ParentId, StaffIds, Enabled)
    {
        this.OriginalId = OriginalId;
        this.IsMigrate = IsMigrate;
    }
}
