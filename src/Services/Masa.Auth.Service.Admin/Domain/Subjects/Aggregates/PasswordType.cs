// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class PasswordType : Enumeration
{
    public static PasswordType Default = new PasswordType();

    public static PasswordType MD5 = new MD5PasswordType();

    public static PasswordType HashPassword = new HashPasswordType();

    public PasswordType() : base(0, "") { }

    public PasswordType(int id, string name) : base(id, name)
    {
    }

    public virtual string EncryptPassword(User user, string password)
    {
        return MD5.EncryptPassword(user, password);
    }

    public virtual bool VerifyPassword(User user, string encryptPassword, string providedPassword)
    {
        return MD5.VerifyPassword(user, encryptPassword, providedPassword);
    }

    public static PasswordType StartNew(string type) => type switch
    {
        nameof(MD5) => new MD5PasswordType(),
        nameof(HashPassword) => new HashPasswordType(),
        _ => new PasswordType()
    };

    private class MD5PasswordType : PasswordType
    {
        public MD5PasswordType() : base(1, nameof(MD5)) { }

        public override string EncryptPassword(User user, string password)
        {
            return MD5Utils.EncryptRepeat(password);
        }

        public override bool VerifyPassword(User user, string encryptPassword, string providedPassword)
        {
            return encryptPassword == MD5Utils.EncryptRepeat(providedPassword ?? "");
        }
    }

    private class HashPasswordType : PasswordType
    {
        public HashPasswordType() : base(2, nameof(HashPassword)) { }

        public override string EncryptPassword(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(user, password);
        }

        public override bool VerifyPassword(User user, string encryptPassword, string providedPassword)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, encryptPassword, providedPassword);
            return result != PasswordVerificationResult.Failed;
        }
    }
}