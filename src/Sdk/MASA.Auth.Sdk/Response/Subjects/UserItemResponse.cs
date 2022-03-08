namespace MASA.Auth.Sdk.Response.Subjects;

public class UserItemResponse
{
    public Guid UserId { get; set; }

    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Avatar { get; private set; }

    public string IDCard { get; private set; }

    public string Account { get; private set; }

    public string Password { get; private set; }

    public string CompanyName { get; private set; }

    public bool Enabled { get; set; }

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

    public UserItemResponse(Guid userId, string name, string displayName, string avatar, string iDCard, string account, string password, string companyName, bool enabled, string phoneNumber, string email, string householdRegisterAddress, string householdRegisterProvinceCode, string householdRegisterCityCode, string householdRegisterDistrictCode, string residentialAddress, string residentialProvinceCode, string residentialCityCode, string residentialDistrictCode)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = iDCard;
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

    #endregion
}

