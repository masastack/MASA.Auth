// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class RegularHelper
{
    public const string CHINESE = "^\\s{0}$|^[\u4e00-\u9fa5]+$";
    public const string NUMBER = "^\\s{0}$|^[0-9]+$";
    public const string LETTER = "^\\s{0}$|^[a-zA-Z]+$";
    public const string LOWER_LETTER = "^\\s{0}$|^[a-z]+$";
    public const string UPPER_LETTER = "^\\s{0}$|^[A-Z]+$";
    public const string LETTER_NUMBER = "^\\s{0}$|^[a-zA-Z0-9]+$";
    public const string CHINESE_LETTER_NUMBER = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9]+$";
    public const string CHINESE_LETTER = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z]+$";
    public const string PHONE = @"^\s{0}$|^((\+86)|(86))?(1[3-9][0-9])\d{8}$";
    public const string IDCARD = "^\\s{0}$|(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    public const string EMAIL = @"^\s{0}$|^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    public const string URL = "^\\s{0}$|[a-zA-z]+://[^s]*";
    public const string PORT = "^\\s{0}$|^([1-9]|[1-9]\\d|[1-9]\\d{2}|[1-9]\\d{3}|[1-5]\\d{4}|6[0-4]\\d{3}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])$";
}

