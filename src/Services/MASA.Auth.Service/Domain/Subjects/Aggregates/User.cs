namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Avatar { get; private set; }

    public string IDCard { get; private set; }

    public string Account { get; private set; }

    public string Password { get; private set; }

    public string CompanyName { get; private set; }


    #region Contact Property

    public string PhoneNumber { get; private set; }

    public string Email { get; private set; }

    public string HouseholdRegisterAddress { get; private set; } = "";

    public string HouseholdRegisterProvinceCode { get; private set; } = "";

    public string HouseholdRegisterCityCode { get; private set; } = "";

    public string HouseholdRegisterDistrictCode { get; private set; } = "";

    public string ResidentialAddress { get; private set; } = "";

    public string ResidentialProvinceCode { get; private set; } = "";

    public string ResidentialCityCode { get; private set; } = "";

    public string ResidentialDistrictCode { get; private set; } = "";

    #endregion

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, string phoneNumber, string email)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = idCard;
        Account = account;
        Password = password;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

}

