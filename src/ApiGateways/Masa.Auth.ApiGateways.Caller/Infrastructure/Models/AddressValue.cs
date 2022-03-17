namespace Masa.Auth.ApiGateways.Caller.Infrastructure.Models;

public class AddressValue 
{
    public string Address { get; set; } = string.Empty;

    public string ProvinceCode { get; set; } = string.Empty;

    public string CityCode { get; set; } = string.Empty;

    public string DistrictCode { get; set; } = string.Empty;

    public AddressValue()
    {

    }

    public AddressValue(string address, string provinceCode, string cityCode, string districtCode)
    {
        Address = address;
        ProvinceCode = provinceCode;
        CityCode = cityCode;
        DistrictCode = districtCode;
    }
}
