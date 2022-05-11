// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared.Default;

public class DefaultNumberTextField<TValue> : MTextField<TValue>
{
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = false;
        Dense = true;
        HideDetails = false;
        Outlined = true;
        Type = "number";

        await base.SetParametersAsync(parameters);
    }
}
