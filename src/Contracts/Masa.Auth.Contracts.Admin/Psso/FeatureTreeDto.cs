// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Psso;

public class FeatureTreeDto
{
    public string Id { set; get; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public FeatureType FeatureType { get; set; }
    public int OrderIndex { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string NamePinyin { get; set; } = string.Empty;
    /// <summary>
    /// 是否折叠
    /// </summary>
    public bool? IsCollapse { get; set; }
    /// <summary>
    /// 是否在演示中显示
    /// </summary>
    public bool? IsShowYS { get; set; }
    public string ParentCode { set; get; } = string.Empty;
    public IEnumerable<FeatureTreeDto> Children { set; get; } = new List<FeatureTreeDto>();
}

public enum FeatureType
{
    /// <summary>
    /// 隐藏
    /// </summary>
    Display = 0,
    /// <summary>
    /// 应用
    /// </summary>
    App = 1,
    /// <summary>
    /// 模块
    /// </summary>
    Module = 2,
    /// <summary>
    /// 菜单
    /// </summary>
    Menu = 3,
    /// <summary>
    /// 设置
    /// </summary>
    Setting = 4,
}