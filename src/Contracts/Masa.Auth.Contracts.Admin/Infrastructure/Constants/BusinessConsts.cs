﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public static class BusinessConsts
{
    public const int PERMISSION_ORDER_MAX_VALUE = 9999;
    public const int PERMISSION_ORDER_MIN_VALUE = 0;
    public const string PASSWORD_REGULAR = @"^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$";
    public const string SWAGGER_TOKEN = "swagger_token";
    public const string I18N_KEY = "$public.i18n.";
}
