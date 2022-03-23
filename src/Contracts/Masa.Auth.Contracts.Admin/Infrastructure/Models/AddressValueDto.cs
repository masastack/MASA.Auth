namespace Masa.Auth.Contracts.Admin.Infrastructure.Models;

public class AddressValueDto
{
    public string Address { get; set; } = string.Empty;

    public string ProvinceCode { get; set; } = string.Empty;

    public string CityCode { get; set; } = string.Empty;

    public string DistrictCode { get; set; } = string.Empty;

    public AddressValueDto()
    {

    }

    public AddressValueDto(string address, string provinceCode, string cityCode, string districtCode)
    {
        Address = address;
        ProvinceCode = provinceCode;
        CityCode = cityCode;
        DistrictCode = districtCode;
    }
}
