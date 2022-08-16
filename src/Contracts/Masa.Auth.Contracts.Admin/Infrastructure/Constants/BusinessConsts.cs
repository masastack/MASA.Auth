// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public static class BusinessConsts
{
    public const int PERMISSION_ORDER_MAX_VALUE = 9999;
    public const int PERMISSION_ORDER_MIN_VALUE = 0;
    public const string PASSWORD_REGULAR = @"^\s{0}$|^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$";
}
