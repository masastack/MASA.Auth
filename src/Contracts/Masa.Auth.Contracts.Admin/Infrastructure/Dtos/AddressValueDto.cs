namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class AddressValueDto
{
    public string Address { get; set; }

    public string ProvinceCode { get; set; }

    public string CityCode { get; set; }

    public string DistrictCode { get; set; }

    public AddressValueDto()
    {
        Address = "";
        ProvinceCode = "";
        CityCode = "";
        DistrictCode = "";
    }

    public AddressValueDto(string address, string provinceCode, string cityCode, string districtCode)
    {
        Address = address;
        ProvinceCode = provinceCode;
        CityCode = cityCode;
        DistrictCode = districtCode;
    }
}
