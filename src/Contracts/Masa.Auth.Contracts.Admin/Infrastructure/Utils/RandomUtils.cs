﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Text;

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class RandomUtils
{
    private const string LETTERS = "ABCDEFGHIJKMLNOPQRSTUVWXYZabcdefghigklmnopqrstuvwxyz";

    private const string NUMBERS = "0123456789";

    public static string GenerateSpecifiedString(int length, bool includeNumbers = false)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            var index = Random.Shared.Next(LETTERS.Length);

            sb.Append(LETTERS[index]);

            if (includeNumbers)
            {
                index = Random.Shared.Next(NUMBERS.Length);
                sb.Append(NUMBERS[index]);
            }
        }
        return sb.ToString();
    }
}
