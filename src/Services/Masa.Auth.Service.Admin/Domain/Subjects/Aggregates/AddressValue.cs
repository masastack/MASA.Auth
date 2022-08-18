// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
        return new AddressValueDto(adress.Address, adress.ProvinceCode, adress.CityCode, adress.DistrictCode);
    }

    [return: NotNullIfNotNull("address")]
    public static implicit operator AddressValue?(AddressValueDto? address)
    {
        if (address is null) return null;
        return new AddressValue(address.Address, address.ProvinceCode, address.CityCode, address.DistrictCode);
    }
}
