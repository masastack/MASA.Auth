// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UserService(ICaller callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/user/";
    }

    public async Task<PaginationDto<UserDto>> GetListAsync(GetUsersDto request)
    {
        return await SendAsync<GetUsersDto, PaginationDto<UserDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<UserSelectDto>> GetSelectAsync(string search)
    {
        return await SendAsync<object, List<UserSelectDto>>(nameof(GetSelectAsync), new { search });
    }

    public async Task<UserDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, UserDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddUserDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateUserDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task UpdateAuthorizationAsync(UpdateUserAuthorizationDto request)
    {
        await SendAsync(nameof(UpdateAuthorizationAsync), request);
    }

    public async Task ResetUserPasswordAsync(ResetUserPasswordDto request)
    {
        await PutAsync(nameof(ResetUserPasswordAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveUserDto(id));
    }

    public async Task<bool> VerifyUserRepeatAsync(VerifyUserRepeatDto user)
    {
        return await PostAsync<VerifyUserRepeatDto, bool>(nameof(VerifyUserRepeatAsync), user);
    }
}

