namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserPaginationQuery(int PageIndex, int PageSize, string Name, string PhoneNumber, string Email, bool Enabled) : Query<PaginationDto<UserDto>>
{
    public override PaginationDto<UserDto> Result { get; set; } = null!;
}
