// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso;

public static class Extensions
{
    /// <summary>
    /// Determines if the authentication scheme support signout.
    /// </summary>
    public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
    {
        var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        var handler = await provider.GetHandlerAsync(context, scheme);
        return (handler is IAuthenticationSignOutHandler);
    }

    /// <summary>
    /// Checks if the redirect URI is for a native client.
    /// </summary>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    /// <summary>
    /// Renders a loading page that is used to redirect back to the redirectUri.
    /// </summary>
    public static IActionResult LoadingPage(this PageModel page, string redirectUri)
    {
        page.HttpContext.Response.StatusCode = 200;
        page.HttpContext.Response.Headers["Location"] = "";

        return page.RedirectToPage("/redirect/index", new { RedirectUri = redirectUri });
    }

    public static void LoadingPage(this NavigationManager nav, string redirectUri)
    {
        var url = nav.GetUriWithQueryParameters("/redirect/index", new Dictionary<string, object?>
        {
            { "RedirectUri", redirectUri }
        });
        nav.NavigateTo(url);
    }

    public static IActionResult LoadingPage(this Controller controller, string redirectUri)
    {
        controller.HttpContext.Response.StatusCode = 200;
        controller.HttpContext.Response.Headers["Location"] = "";

        return controller.RedirectToPage("/redirect/index", new { RedirectUri = redirectUri });
    }

    public static void NavigateTo(this NavigationManager nav, string uri, Dictionary<string, object?> parameters)
    {
        var url = nav.GetUriWithQueryParameters(uri, parameters);
        nav.NavigateTo(url);
    }
}
