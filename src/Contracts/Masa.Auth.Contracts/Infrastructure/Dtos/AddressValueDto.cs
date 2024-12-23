// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
