// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Psso;

public class AssignedRoleDto
{
    public int? RoleId { set; get; }
    public PssoRoleType? RoleType { set; get; }
    public string RoleName { set; get; } = string.Empty;
    public string AssociatedEntityId { set; get; } = string.Empty;
    public string AssociatedEntityName { get; set; } = string.Empty;
}

public enum PssoRoleType : int
{
    /// <summary>
    /// 默认
    /// </summary>
    Default = 0,
    /// <summary>
    /// 经销商
    /// </summary>
    Distributor = 1,
    /// <summary>
    /// 经销公司
    /// </summary>
    DistributionCompany = 2,
    ///供应商
    Supplier = 3,
    /// <summary>
    /// 服务中心
    /// </summary>
    ServiceCenter = 8,
    ///// <summary>
    ///// 技工技师
    ///// </summary>
    //[Display(Name = "技工技师")]
    //Worker = 10,
    ///// <summary>
    ///// 临时技工技师
    ///// </summary>
    //[Display(Name = "临时技工技师")]
    //TempWorker = 11,
}