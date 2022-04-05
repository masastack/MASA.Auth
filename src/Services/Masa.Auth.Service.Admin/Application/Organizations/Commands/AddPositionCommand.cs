namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public record AddPositionCommand(AddPositionDto Position) : Command
{
    public Guid PositionId { get; set; }
}

