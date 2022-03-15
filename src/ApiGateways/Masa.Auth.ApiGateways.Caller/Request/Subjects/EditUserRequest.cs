using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class EditUserRequest
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    #region Contact Property

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

    public EditUserRequest(Guid userId, string name, string displayName, string avatar, string companyName, bool enabled, string email, string householdRegisterAddress, string householdRegisterProvinceCode, string householdRegisterCityCode, string householdRegisterDistrictCode, string residentialAddress, string residentialProvinceCode, string residentialCityCode, string residentialDistrictCode)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        CompanyName = companyName;
        Enabled = enabled;
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

    public static implicit operator EditUserRequest(UserItemResponse user)
    {
        return new EditUserRequest(user.UserId, user.Name, user.DisplayName, user.Avatar, user.CompanyName, user.Enabled, user.Email, user.HouseholdRegisterAddress, user.HouseholdRegisterProvinceCode, user.HouseholdRegisterCityCode, user.HouseholdRegisterDistrictCode, user.ResidentialAddress, user.ResidentialProvinceCode, user.ResidentialCityCode, user.ResidentialDistrictCode);
    }
}
