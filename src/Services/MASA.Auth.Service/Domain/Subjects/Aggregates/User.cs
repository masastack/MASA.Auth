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

    public bool Enabled { get; private set; }

    #region Contact Property

    public string PhoneNumber { get; private set; }

    public string Email { get; private set; }

    public string HouseholdRegisterAddress { get; private set; } = string.Empty;

    public string HouseholdRegisterProvinceCode { get; private set; } = string.Empty;

    public string HouseholdRegisterCityCode { get; private set; } = string.Empty;

    public string HouseholdRegisterDistrictCode { get; private set; } = string.Empty;

    public string ResidentialAddress { get; private set; } = string.Empty;

    public string ResidentialProvinceCode { get; private set; } = string.Empty;

    public string ResidentialCityCode { get; private set; } = string.Empty;

    public string ResidentialDistrictCode { get; private set; } = string.Empty;


    #endregion

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, bool enabled, string phoneNumber, string email, string householdRegisterAddress, string householdRegisterProvinceCode, string householdRegisterCityCode, string householdRegisterDistrictCode, string residentialAddress, string residentialProvinceCode, string residentialCityCode, string residentialDistrictCode)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = idCard;
        Account = account;
        Password = password;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        HouseholdRegisterAddress = householdRegisterAddress;
        HouseholdRegisterProvinceCode = householdRegisterProvinceCode;
        HouseholdRegisterCityCode = householdRegisterCityCode;
        HouseholdRegisterDistrictCode = householdRegisterDistrictCode;
        ResidentialAddress = residentialAddress;
        ResidentialProvinceCode = residentialProvinceCode;
        ResidentialCityCode = residentialCityCode;
        ResidentialDistrictCode = residentialDistrictCode;
    }

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, bool enabled, string phoneNumber, string email)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = idCard;
        Account = account;
        Password = password;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}

