namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserSelectDto : AutoCompleteDocument<Guid>
{
    public new Guid Id { get; set; }

    public string Name { get; set; }

    public string Account { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Avatar { get; set; }

    public UserSelectDto()
    {
        Name = "";
        Account = "";
        PhoneNumber = "";
        Email = "";
        Avatar = "";
    }

    public UserSelectDto(Guid id, string name, string account, string phoneNumber, string email, string avatar)
    {
        Id = id;
        Value = Id;
        Name = name;
        Account = account;
        PhoneNumber = phoneNumber;
        Email = email;
        Avatar = avatar;
    }

    protected override string IdGenerator()
    {
        return $"{Name}{Account}{PhoneNumber}{Email}";
    }
}
