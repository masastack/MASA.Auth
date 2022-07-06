// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Logs;

public class OperationLogDto
{
    public Guid Id { get; set; }

    public Guid Operator { get; set; }

    public string OperatorName { get; set; } = "";

    public OperationTypes OperationType { get; set; }

    public DateTime OperationTime { get; set; }

    public string OperationDescription { get; set; } = "";

    public OperationLogDto() { }

    public OperationLogDto(
        Guid id, 
        Guid @operator, 
        string operatorName,
        OperationTypes operationType,
        DateTime operationTime, 
        string operationDescription)
    {
        Id = id;
        Operator = @operator;
        OperatorName = operatorName;
        OperationType = operationType;
        OperationTime = operationTime;
        OperationDescription = operationDescription;
    }
}

