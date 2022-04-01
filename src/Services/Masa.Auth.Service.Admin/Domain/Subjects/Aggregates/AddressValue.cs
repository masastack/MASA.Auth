namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class AddressValue : ValueObject
{
    public string Address { get; private set; }

    public string ProvinceCode { get; private set; }

    public string CityCode { get; private set; }

    public string DistrictCode { get; private set; }

    public AddressValue()
    {
        Address = "";
        ProvinceCode = "";
        CityCode = "";
        DistrictCode = "";
    }

    public AddressValue(string address, string provinceCode, string cityCode, string districtCode)
    {
        Address = address;
        ProvinceCode = provinceCode;
        CityCode = cityCode;
        DistrictCode = districtCode;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Address;
        yield return ProvinceCode;
        yield return CityCode;
        yield return DistrictCode;
    }

    public static implicit operator AddressValueDto(AddressValue adress)
    {
        return new AddressValueDto(adress.Address,adress.ProvinceCode,adress.CityCode,adress.DistrictCode);
    }

    public static implicit operator AddressValue(AddressValueDto adress)
    {
        return new AddressValue(adress.Address, adress.ProvinceCode, adress.CityCode, adress.DistrictCode);
    }
}
