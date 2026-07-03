// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityModel;

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserPasswordCommandValidator : MasaAbstractValidator<UpdateUserPasswordCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateUserPasswordCommandValidator(
        IHttpContextAccessor httpContextAccessor,
        IPasswordRuleProvider passwordRuleProvider)
    {
        _httpContextAccessor = httpContextAccessor;

        RuleFor(command => command.User.NewPassword).PasswordRule(passwordRuleProvider, _ => ResolveCurrentClientId());
        RuleFor(command => command.User.NewPassword)
                .Required()
                .NotEqual(command => command.User.OldPassword)
                .WithMessage(I18n.T("PasswordSame"));
    }

    // The user is already logged in here, so the client-specific rule follows the client they're
    // currently using (the access token's client_id claim) - not User.ClientId, which only records
    // where the account originally registered from.
    private string? ResolveCurrentClientId()
        => _httpContextAccessor.HttpContext?.User?.FindFirst(JwtClaimTypes.ClientId)?.Value;
}
