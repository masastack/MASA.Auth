// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class RandomUtils
{
    private const string LETTERS = "ABCDEFGHIJKMLNOPQRSTUVWXYZabcdefghigklmnopqrstuvwxyz";
    private const string LETTERNUMBERS = "ABCDEFGHIJKMLNOPQRSTUVWXYZabcdefghigklmnopqrstuvwxyz0123456789";

    public static string GenerateSpecifiedString(int length, bool includeNumbers = false)
    {
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            if (includeNumbers)
            {
                var index = Random.Shared.Next(LETTERNUMBERS.Length);
                sb.Append(LETTERNUMBERS[index]);
            }
            else
            {
                var index = Random.Shared.Next(LETTERS.Length);
                sb.Append(LETTERS[index]);
            }
        }
        return sb.ToString();
    }
}
