namespace Masa.Auth.Sdk.Request.Subjects;

public class AddUserRequest
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IDCard { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    #region Contact Property

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string HouseholdRegisterAddress { get; set; }

    public string HouseholdRegisterProvinceCode { get; set; }

    public string HouseholdRegisterCityCode { get; set; }

    public string HouseholdRegisterDistrictCode { get; set; }

    public string ResidentialAddress { get; set; }

    public string ResidentialProvinceCode { get; set; }

    public string ResidentialCityCode { get; set; }

    public string ResidentialDistrictCode { get; set; }

    #endregion

    public AddUserRequest(string name, string displayName, string avatar, string iDCard, string companyName, bool enabled, string phoneNumber, string email, string householdRegisterAddress, string householdRegisterProvinceCode, string householdRegisterCityCode, string householdRegisterDistrictCode, string residentialAddress, string residentialProvinceCode, string residentialCityCode, string residentialDistrictCode)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = iDCard;
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

    public static implicit operator AddUserRequest(UserItemResponse user)
    {
        return new AddUserRequest(user.Name, user.DisplayName, user.Avatar, user.IDCard, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.HouseholdRegisterAddress, user.HouseholdRegisterProvinceCode, user.HouseholdRegisterCityCode, user.HouseholdRegisterDistrictCode, user.ResidentialAddress, user.ResidentialProvinceCode, user.ResidentialCityCode, user.ResidentialDistrictCode);
    }
}
