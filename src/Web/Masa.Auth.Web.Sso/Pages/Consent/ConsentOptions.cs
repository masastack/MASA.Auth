// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Consent;

public class ConsentOptions
{
    public static bool EnableOfflineAccess = true;
    public static string OfflineAccessDisplayName = "Offline Access";
    public static string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";

    public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";
}