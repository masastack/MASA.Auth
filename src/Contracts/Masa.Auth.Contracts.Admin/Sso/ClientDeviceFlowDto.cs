// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientDeviceFlowDto
{
    public string UserCodeType { get; set; } = string.Empty;

    public int DeviceCodeLifetime { get; set; }

    public static implicit operator ClientDeviceFlowDto(ClientDetailDto model)
    {
        return new ClientDeviceFlowDto()
        {
            UserCodeType = model.UserCodeType,
            DeviceCodeLifetime = model.DeviceCodeLifetime
        };
    }
}
