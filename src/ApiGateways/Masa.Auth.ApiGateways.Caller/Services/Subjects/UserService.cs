﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.
using static Masa.Auth.Contracts.Admin.UserClaimValuesDto;

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UserService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/user/";
    }

    public async Task<PaginationDto<UserDto>> GetListAsync(GetUsersDto request)
    {
        return await SendAsync<GetUsersDto, PaginationDto<UserDto>>("list", request);
    }

    public async Task<List<UserSelectDto>> GetSelectAsync(string search)
    {
        return await SendAsync<object, List<UserSelectDto>>("select", new { search });
    }

    public async Task<UserDetailDto> GetDetailAsync(Guid id)
    {
        return await GetAsync<UserDetailDto>($"detail/{id}");
    }

    public async Task AddAsync(AddUserDto request)
    {
        await PostAsync("", request);
    }

    public async Task UpdateAsync(UpdateUserDto request)
    {
        await PutAsync("", request);
    }

    public async Task UpdateAuthorizationAsync(UpdateUserAuthorizationDto request)
    {
        await PutAsync("authorization", request);
    }

    public async Task ResetPasswordAsync(ResetUserPasswordDto request)
    {
        await PutAsync(nameof(ResetPasswordAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await DeleteAsync("", new RemoveUserDto(id));
    }

    public async Task<bool> VerifyRepeatAsync(VerifyUserRepeatDto user)
    {
        return await GetAsync<VerifyUserRepeatDto, bool>(nameof(VerifyRepeatAsync), user);
    }

    public async Task<List<ClaimValue>> GetClaimValuesAsync(Guid userId)
    {
        return await GetAsync<List<ClaimValue>>($"ClaimValues/{userId}");
    }

    public async Task SaveClaimValuesAsync(UserClaimValuesDto userClaimValuesDto)
    {
        await PostAsync(nameof(SaveClaimValuesAsync), userClaimValuesDto);
    }
}

