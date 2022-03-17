namespace Masa.Auth.Service.Admin.Application.Subjects.Models;

public class UserSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;
}
