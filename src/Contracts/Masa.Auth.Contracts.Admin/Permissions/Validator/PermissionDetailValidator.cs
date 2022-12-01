// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions.Validator;

public class PermissionDetailValidator<T> : AbstractValidator<T> where T : PermissionDetailDto
{
    public PermissionDetailValidator()
    {
        RuleFor(p => p).NotNull().WithMessage($"Parameter error");
        RuleFor(p => p.Name).ChineseLetterNumber().Must(name => !string.IsNullOrWhiteSpace(name) && name.Length <= 20)
            .WithMessage("Permission Name can`t null and length must be less than 20.");
        RuleFor(p => p.Description).ChineseLetterNumber().Must(description => description.Length <= 255)
            .WithMessage("Permission Description length must be less than 255.");
        RuleFor(p => p.AppId).Required();
        RuleFor(p => p.Code).Required();
        RuleFor(p => p.Order).NotNull().Must(order => order >= BusinessConsts.PERMISSION_ORDER_MIN_VALUE
            && order <= BusinessConsts.PERMISSION_ORDER_MAX_VALUE)
            .WithMessage($"The valid value of order is between {BusinessConsts.PERMISSION_ORDER_MIN_VALUE}-{BusinessConsts.PERMISSION_ORDER_MAX_VALUE}");
    }
}