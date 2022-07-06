// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Logs;

public class OperationLogDetailDto : OperationLogDto
{
    public OperationLogDetailDto() { }

    public OperationLogDetailDto(
        Guid id, 
        Guid @operator, 
        string operatorName, 
        OperationTypes operationType, 
        DateTime operationTime, 
        string operationDescription
        ) : base(id, @operator, operatorName, operationType, operationTime, operationDescription)
    {
    }
}

