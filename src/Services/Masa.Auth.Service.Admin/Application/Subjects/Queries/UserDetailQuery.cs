using Masa.Auth.Service.Admin.Application.Subjects.Models;

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserDetailQuery(Guid UserId) : Query<UserDetail>
{
    public override UserDetail Result { get; set; } = null!;
}
