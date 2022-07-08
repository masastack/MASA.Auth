// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Logs.Aggregates
{
    public class OperationLog : AggregateRoot<Guid>
    {
        public Guid Operator { get; private set; }

        public string OperatorName { get; private set; } = "";

        public OperationTypes OperationType { get; private set; }

        public DateTime OperationTime { get; private set; }

        public string OperationDescription { get; private set; } = "";

        public string TraceIdentifier { get; private set; } = "";
    }
}
