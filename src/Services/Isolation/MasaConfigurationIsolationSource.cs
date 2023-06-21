// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace Masa.Contrib.StackSdks.Isolation;

internal class MasaConfigurationIsolationSource : IConfigurationSource
{
    readonly DccConfigurationIsolationRepository _configurationIsolationRepository;

    public MasaConfigurationIsolationSource(DccConfigurationIsolationRepository configurationIsolationRepository)
    {
        _configurationIsolationRepository = configurationIsolationRepository;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new MasaConfigurationIsolationProvider(_configurationIsolationRepository);
}
