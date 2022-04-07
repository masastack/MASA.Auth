namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserSelectQuery(string Search) : Query<List<UserSelectDto>>
{
    public override List<UserSelectDto> Result { get; set; } = new();
}
