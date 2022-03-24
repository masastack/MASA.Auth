namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public UserSelectDto(Guid id, string name, string avatar)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
    }
}
