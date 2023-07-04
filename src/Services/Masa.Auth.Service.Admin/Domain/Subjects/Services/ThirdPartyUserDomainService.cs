// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class ThirdPartyUserDomainService : DomainService
{
    readonly AuthDbContext _authDbContext;
    readonly UserDomainService _userDomainService;

    public ThirdPartyUserDomainService(UserDomainService userDomainService, AuthDbContext authDbContext)
    {
        _userDomainService = userDomainService;
        _authDbContext = authDbContext;
    }

    public async Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserDto dto)
    {
        var userDto = dto.User;
        var user = new User(userDto.Name, userDto.DisplayName ?? "", userDto.Avatar, userDto.Account, userDto.Password, "", userDto.Email, userDto.PhoneNumber ?? "",
             new ThirdPartyUser(dto.ThirdPartyIdpId, true, dto.ThridPartyIdentity, dto.ExtendedData));
        await _userDomainService.AddAsync(user);
        return user.Adapt<UserModel>();
    }

    public async Task<ThirdPartyUser?> VerifyRepeatAsync(Guid thirdPartyIdpId, string thridPartyIdentity)
    {
        var thirdPartyUser = await _authDbContext.Set<ThirdPartyUser>()
                                                 .Include(tpu => tpu.User)
                                                 .ThenInclude(user => user.Roles)
                                                 .FirstOrDefaultAsync(tpu => tpu.ThirdPartyIdpId == thirdPartyIdpId && tpu.ThridPartyIdentity == thridPartyIdentity);
        return thirdPartyUser;
    }
}
