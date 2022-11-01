// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class SyncStaffDto
{
    [ImporterHeader(Name = "姓名")]
    public string? Name { get; set; }

    [ImporterHeader(Name = "工号(必填)")]
    public string JobNumber { get; set; } = "";

    [ImporterHeader(Name = "手机号码(必填)")]
    public string PhoneNumber { get; set; } = "";

    [ImporterHeader(Name = "邮箱")]
    public string? Email { get; set; }

    [ImporterHeader(Name = "身份证号码")]
    public string? IdCard { get; set; }

    [ImporterHeader(Name = "昵称(必填)")]
    public string DisplayName { get; set; } = "";

    [ImporterHeader(Name = "岗位")]
    public string? Position { get; set; }

    [ImporterHeader(IsIgnore = true)]
    public GenderTypes Gender
    {
        get
        {
            if (GenderTypeLable is "男" or "Male") return GenderTypes.Male;
            else return GenderTypes.Female;
        }
    }

    [ImporterHeader(Name = "性别")]
    public string GenderTypeLable { get; set; } = GenderTypes.Male.ToString();

    [ImporterHeader(IsIgnore = true)]
    public StaffTypes StaffType
    {
        get
        {
            if (StaffTypeLable is "内部员工" or "Internal") return StaffTypes.Internal;
            else return StaffTypes.External;
        }
    }

    [ImporterHeader(Name = "员工类型")]
    public string StaffTypeLable { get; set; } = StaffTypes.Internal.ToString();
}


