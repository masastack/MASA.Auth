// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs.Commands;

public record AddOperationLogCommand(
    Guid Operator,
    string OperatorName,
    OperationTypes OperationType,
    DateTime OperationTime,
    string OperationDescription
    ) : Command
{
}
