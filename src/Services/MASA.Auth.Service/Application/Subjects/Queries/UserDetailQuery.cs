namespace Masa.Auth.Service.Application.Subjects.Queries;

public record UserDetailQuery(Guid UserId) : Query<UserDetail>
{
    public override UserDetail Result { get; set; } = null!;
}
