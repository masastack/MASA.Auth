// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class MasaStackConfigExtensions
{
    public static string GetDbType(this IMasaStackConfig masaStackConfig)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConstant.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);
        return dbModel?.DbType ?? string.Empty;
    }
}
