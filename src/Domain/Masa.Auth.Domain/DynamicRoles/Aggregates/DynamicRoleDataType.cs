// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class DynamicRoleDataType : Enumeration
{
    public static DynamicRoleDataType Default = new DynamicRoleDataType();

    public static DynamicRoleDataType UserInfo = new UserInfoData();

    public static DynamicRoleDataType UserClaim = new UserClaimData();

    public DynamicRoleDataType() : base(0, "") { }

    public DynamicRoleDataType(int id, string name) : base(id, name)
    {
    }

    public virtual string? GetValueFromUser(User user, string fieldName)
    {
        throw new NotImplementedException();
    }

    public static DynamicRoleDataType StartNew(string type) => type switch
    {
        nameof(UserInfo) => new UserInfoData(),
        nameof(UserClaim) => new UserClaimData(),
        _ => new DynamicRoleDataType()
    };

    private class UserInfoData : DynamicRoleDataType
    {
        public UserInfoData() : base(1, nameof(UserInfo)) { }

        public override string? GetValueFromUser(User user, string fieldName)
        {
            var property = typeof(User).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                var value = property.GetValue(user);
                return value?.ToString();
            }
            return null;
        }
    }

    private class UserClaimData : DynamicRoleDataType
    {
        public UserClaimData() : base(2, nameof(UserClaim)) { }

        public override string? GetValueFromUser(User user, string fieldName)
        {
            return user.UserClaims.FirstOrDefault(x => x.Name == fieldName)?.Value;
        }
    }
}
