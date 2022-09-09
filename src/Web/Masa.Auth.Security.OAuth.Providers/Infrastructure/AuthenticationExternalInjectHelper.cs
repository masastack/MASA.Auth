// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class AuthenticationExternalInjectHelper
{
    readonly static List<IAuthenticationExternalInject> authenticationExternalInjects;

    static AuthenticationExternalInjectHelper()
    {
        var injectTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsInterface is false && type.IsAssignableTo(typeof(IAuthenticationExternalInject)));

        authenticationExternalInjects = injectTypes.Select(type => (IAuthenticationExternalInject)type.Assembly.CreateInstance(type.FullName!)!)
                                       .ToList();
    }

    public static void Inject(this AuthenticationBuilder builder, params AuthenticationDefaults[] authenticationDefaults)
    {
        foreach (var item in authenticationDefaults)
        {
            var inject = authenticationExternalInjects.FirstOrDefault(inject => inject.Scheme == item.Scheme);
            if (inject is not null) inject.Inject(builder, item);
            else
            {
                builder.AddOAuth(item.Scheme, item.DisplayName, options =>
                {
                    item.BindOAuthOptions(options);
                });
            }
            //todo support builder.AddOpenIdConnect()
        }
    }
}
