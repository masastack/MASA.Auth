// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs.Commands;

public class AddOperationLogCommandValidator : AbstractValidator<AddOperationLogCommand>
{
    public AddOperationLogCommandValidator()
    {
        RuleFor(staff => staff.Operator).Required();
        RuleFor(staff => staff.OperatorName).Required();
        RuleFor(staff => staff.OperationType).Required();
        RuleFor(staff => staff.OperationTime).Required();
    }
}
