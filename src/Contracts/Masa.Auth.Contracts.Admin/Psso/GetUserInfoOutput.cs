// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Psso;

/// <summary>
/// 获取用户信息
/// </summary>
public class GetUserInfoOutput
{
    public Guid Id { get; set; }
    public Guid LonsidUserId { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { set; get; } = string.Empty;
    /// <summary>
    /// 租户ID
    /// </summary>
    public int? TenantId { set; get; }
    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid? UnitId { set; get; }
    /// <summary>
    /// 经销公司ID
    /// </summary>
    public string CompanyId { set; get; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string DistributorId { get; set; } = string.Empty;
    public string DistributorName { get; set; } = string.Empty;
    public string DomainName { set; get; } = string.Empty;
    public Guid? SystemUserId { get; set; }
    public Guid? RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string RegionOwner { get; set; } = string.Empty;
    public string ServiceCenterId { get; set; } = string.Empty;
    public string ServiceCenterName { get; set; } = string.Empty;
    public bool? IsPaymentYS { get; set; }
    public bool? IsExclusiveStore { get; set; }
}