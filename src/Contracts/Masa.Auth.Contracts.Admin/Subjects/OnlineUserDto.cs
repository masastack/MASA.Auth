// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public record OnlineUserDto(
    Guid UserId,
    string Account,
    string DisplayName,
    string Avatar,
    WebSessionDto? WebSession);

public record WebSessionDto(DateTime LoginTime, string? ClientId);

public record KickUserModel(string SubjectId);

public class GetOnlineUsersDto : Pagination<GetOnlineUsersDto>
{
    public string? Search { get; set; }
}
