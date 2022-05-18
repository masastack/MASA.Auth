// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class SyncStaffDto
{
    [ExporterHeader(DisplayName = "序号")]
    public int Index { get; set; }

    [ExporterHeader(DisplayName = "姓名")]
    public string Name { get; set; } = "";

    [ExporterHeader(DisplayName = "账号")]
    public string Account { get; set; } = "";

    [ExporterHeader(DisplayName = "密码")]
    public string Password { get; set; } = "";

    [ExporterHeader(DisplayName = "工号")]
    public string JobNumber { get; set; } = "";

    [ExporterHeader(DisplayName = "手机号")]
    public string? PhoneNumber { get; set; }

    [ExporterHeader(DisplayName = "邮箱")]
    public string? Email { get; set; }

    [ExporterHeader(DisplayName = "身份证号")]
    public string? IdCard { get; set; }

    [ExporterHeader(DisplayName = "昵称")]
    public string? DisplayName { get; set; }

    [ExporterHeader(DisplayName = "岗位")]
    public string? Position { get; set; }

    [ExporterHeader(IsIgnore = true)]
    public GenderTypes GenderType => Enum.Parse<GenderTypes>(GenderTypeLable);

    [ExporterHeader(DisplayName = "性别")]
    public string GenderTypeLable { get; set; } = GenderTypes.Male.ToString();

    [ExporterHeader(IsIgnore = true)]
    public StaffTypes StaffType => Enum.Parse<StaffTypes>(StaffTypeLable);

    [ExporterHeader(DisplayName = "员工类型")]
    public string StaffTypeLable { get; set; } = StaffTypes.InternalStaff.ToString();

    //[ExporterHeader(DisplayName = "备注")]
    //public string? Remark { get; set; }
}


