// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions.Validator;

public class PermissionDetailDtoValidator<T> : AbstractValidator<T> where T : PermissionDetailDto
{
    public PermissionDetailDtoValidator()
    {
        RuleFor(p => p).NotNull().WithMessage($"Parameter error");
        RuleFor(p => p.Name).ChineseLetterNumber().Must(name => !string.IsNullOrWhiteSpace(name) && name.Length <= 20)
            .WithMessage("Department Name can`t null and length must be less than 20.");
        RuleFor(p => p.Description).ChineseLetterNumber().Must(description => description.Length <= 255)
            .WithMessage("Department Description length must be less than 255.");
        RuleFor(p => p.AppId).NotNull().WithMessage("AppId can`t null");
        RuleFor(p => p.Code).NotNull().WithMessage("Code can`t null");
    }
}