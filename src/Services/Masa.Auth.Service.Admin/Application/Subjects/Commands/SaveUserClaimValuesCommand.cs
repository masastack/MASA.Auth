// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static Masa.Auth.Contracts.Admin.UserClaimValuesDto;

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record SaveUserClaimValuesCommand(Guid UserId, List<ClaimValue> ClaimValues) : Command;
