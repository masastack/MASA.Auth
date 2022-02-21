namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
    #region social property

    public string? ChineseName { get; set; }

    public string? EnglishName { get; set; }

    public string? DisplayName { get; set; }

    public string? IdentityNumber { get; set; }

    public string? Company { get; set; }

    public string? PoliticalStatus { get; set; }

    public bool? IsMarried { get; set; }

    #endregion

    #region own property

    public Sex Sex { get; set; }

    public byte? Age { get; set; }

    public DateOnly? BirthDay { get; set; }

    /// <summary>
    /// unit cm
    /// </summary>
    public short? Height { get; set; }

    /// <summary>
    /// unit catty
    /// </summary>
    public short? Weight { get; set; }

    public BloodType BloodType { get; set; }

    #endregion

    #region contact property

    public string? MobilePhone { get; set; }

    public string? OfficePhone { get; set; }

    public string? Email { get; set; }

    public string? WorkingAddress { get; set; }

    public string? WorkingProvinceCode { get; set; }

    public string? WorkingCityCode { get; set; }

    public string? WorkingDistrictCode { get; set; }

    public string? HouseholdRegisterAddress { get; set; }

    public string? HouseholdRegisterProvinceCode { get; set; }

    public string? HouseholdRegisterCityCode { get; set; }

    public string? HouseholdRegisterDistrictCode { get; set; }

    public string? ResidentialAddress { get; set; }

    public string? ResidentialProvinceCode { get; set; }

    public string? ResidentialCityCode { get; set; }

    public string? ResidentialDistrictCode { get; set; }

    #endregion

    #region image

    public string? Photo { get; set; }

    public string? IdentityFrontPhoto { get; set; }

    public string? IdentityBackPhoto { get; set; }

    public string? HouseholdRegisterPhoto { get; set; }

    #endregion
}

public enum Sex
{
    Unknown,
    Man,
    Woman,
}

public enum BloodType
{
    Unknown,
    A,
    B,
    AB,
    O
}

