namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record PositionSelectQuery(string Name) : Query<List<PositionSelectDto>>
{
    public override List<PositionSelectDto> Result { get; set; } = new();
}
