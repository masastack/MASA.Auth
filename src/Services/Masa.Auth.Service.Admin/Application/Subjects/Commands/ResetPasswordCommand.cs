// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record ResetPasswordCommand(ResetPasswordTypes ResetPasswordType, string Voucher, string Captcha) : Command
{
    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// The client the user is resetting their password from. This is an anonymous flow (no login),
    /// so the caller must pass it explicitly - it cannot be resolved from a token.
    /// </summary>
    public string? ClientId { get; set; }

    public User? Result { get; set; }
}
