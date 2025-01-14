namespace Masa.Stack.Components.Models;

public class User
{
    public string Account { get; set; }

    public string? Avatar { get; set; }

    public string DisplayName { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 性别 TODO: use enum?
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    public string? JobNumber { get; set; }

    public string? Email { get; set; }

    public string? Position { get; set; }

    public string? PhoneNumber { get; set; }

    public IEnumerable<string>? Teams { get; set; }

    public string? CompanyName { get; set; }

    /// <summary>
    /// 国家或地区
    /// </summary>
    public string? Region { get; set; }

    // TODO: 地址树
    public string? Address { get; set; }

    // TODO: 部门树
    public string? Department { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    public string GenderText => Gender == 0 ? "Female" : "Male";

    public User Clone()
    {
        return (User)this.MemberwiseClone();
    }
}
