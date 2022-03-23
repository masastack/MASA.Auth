namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserDetailQuery(Guid UserId) : Query<UserDetailDto>
{
    public override UserDetailDto Result { get; set; } = null!;
}
