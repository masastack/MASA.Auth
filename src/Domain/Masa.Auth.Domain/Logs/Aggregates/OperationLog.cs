// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Logs.Aggregates
{
    public class OperationLog : AggregateRoot<Guid>
    {
        Guid _operator;
        string _operatorName = "";
        OperationTypes _operationType;
        DateTime _operationTime;
        string _operationDescription = "";
        string? _clientId;

        public Guid Operator
        {
            get => _operator;
            set => _operator = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(Operator));
        }

        public string OperatorName
        {
            get => _operatorName;
            set => _operatorName = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(OperatorName));
        }

        public OperationTypes OperationType
        {
            get => _operationType;
            set => _operationType = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(OperationType));
        }

        public DateTime OperationTime
        {
            get => _operationTime;
            set
            {
                if (value.Kind == DateTimeKind.Local)
                    _operationTime = value.ToUniversalTime();
                else
                    _operationTime = value;
            }
        }

        [AllowNull]
        public string OperationDescription
        {
            get => _operationDescription;
            set => _operationDescription = value ?? "";
        }

        /// <summary>
        /// Client ID, used to record the client source of the operation
        /// </summary>
        [AllowNull]
        public string? ClientId
        {
            get => _clientId;
            set => _clientId = value;
        }

        /// <summary>
        /// Constructor for creating operation log (without client information)
        /// </summary>
        public OperationLog(Guid @operator, string operatorName, OperationTypes operationType, DateTime operationTime, string? operationDescription)
        {
            Operator = @operator;
            OperatorName = operatorName;
            OperationType = operationType;
            if (operationTime == default) operationTime = DateTime.UtcNow;
            OperationTime = operationTime;
            OperationDescription = operationDescription;
        }

        /// <summary>
        /// Constructor for creating operation log (with client information)
        /// </summary>
        public OperationLog(Guid @operator, string operatorName, OperationTypes operationType, DateTime operationTime, string? operationDescription, string? clientId)
        {
            Operator = @operator;
            OperatorName = operatorName;
            OperationType = operationType;
            if (operationTime == default) operationTime = DateTime.UtcNow;
            OperationTime = operationTime;
            OperationDescription = operationDescription;
            ClientId = clientId;
        }
    }
}
