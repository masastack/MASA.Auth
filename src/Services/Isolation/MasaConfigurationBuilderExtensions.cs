// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
using Microsoft.Extensions.Configuration;

namespace Masa.Contrib.StackSdks.Isolation;

internal static class MasaConfigurationBuilderExtensions
{
    public static IMasaConfigurationBuilder UseDccIsolation(
        this IMasaConfigurationBuilder builder,
        string sectionName = "DccOptions")
    {
        var configurationSection = builder.Configuration.GetSection(sectionName);
        var dccOptions = configurationSection.Get<DccOptions>();
        return builder.UseDccIsolation(dccOptions);
    }

    public static IMasaConfigurationBuilder UseDccIsolation(
        this IMasaConfigurationBuilder builder,
        DccOptions dccOptions)
    {
        builder.UseDcc(dccOptions);
        var dccConfigurationOptions = ComplementDccConfigurationOption(builder, dccOptions);

        var serviceProvider = builder.Services.BuildServiceProvider();

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationIsolationRepository(dccConfigurationOptions.GetAllSections(),
            configurationApiClient, loggerFactory));
        return builder;
    }

    public static DccConfigurationOptions ComplementDccConfigurationOption(
        IMasaConfigurationBuilder builder,
        DccOptions dccOptions)
    {
        DccConfigurationOptions dccConfigurationOptions = dccOptions;
        var serviceProvider = builder.Services.BuildServiceProvider();
        var environmentProvider = serviceProvider.GetRequiredService<EnvironmentProvider>();
        foreach (var environment in environmentProvider.GetEnvionments())
        {
            if (environment != dccConfigurationOptions.DefaultSection.Environment &&
                !dccConfigurationOptions.ExpandSections.Any(section => section.AppId.Equals(dccConfigurationOptions.DefaultSection.AppId)
                && section.Environment.Equals(environment)))
            {
                var publicSection = new DccSectionOptions
                {
                    AppId = dccConfigurationOptions.DefaultSection.AppId,
                    Secret = dccConfigurationOptions.DefaultSection.Secret,
                    Environment = environment,
                };
                dccConfigurationOptions.ExpandSections.Add(publicSection);
            }
            if (!dccConfigurationOptions.ExpandSections.Any(section => section.AppId.Equals("PublicId") && section.Environment.Equals(environment)))
            {
                var publicSection = new DccSectionOptions
                {
                    AppId = "PublicId",
                    Secret = "Secret",
                    Environment = environment,
                };
                dccConfigurationOptions.ExpandSections.Add(publicSection);
            }
        }
        return dccConfigurationOptions;
    }
}
