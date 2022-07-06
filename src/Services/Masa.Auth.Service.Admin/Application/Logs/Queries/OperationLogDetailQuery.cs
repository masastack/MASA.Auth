﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs.Queries;

public record OperationLogDetailQuery(Guid OperationLogId) : Query<OperationLogDetailDto>
{
    public override OperationLogDetailDto Result { get; set; } = new();
}
