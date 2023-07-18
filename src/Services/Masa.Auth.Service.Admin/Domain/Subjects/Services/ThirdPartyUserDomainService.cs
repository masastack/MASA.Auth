// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class ThirdPartyUserDomainService : DomainService
{
    readonly AuthDbContext _authDbContext;
    readonly UserDomainService _userDomainService;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;

    public ThirdPartyUserDomainService(
        UserDomainService userDomainService,
        AuthDbContext authDbContext,
        IThirdPartyUserRepository thirdPartyUserRepository)
    {
        _userDomainService = userDomainService;
        _authDbContext = authDbContext;
        _thirdPartyUserRepository = thirdPartyUserRepository;
    }

    public async Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserDto dto)
    {
        var userDto = dto.User;
        var user = new User(userDto.Name, userDto.DisplayName ?? "", userDto.Avatar, userDto.Account, userDto.Password, "", userDto.Email, userDto.PhoneNumber ?? "",
             new ThirdPartyUser(dto.ThirdPartyIdpId, true, dto.ThridPartyIdentity, dto.ExtendedData));
        var (existUser, _) = await _userDomainService.VerifyRepeatAsync(userDto.PhoneNumber, userDto.Email, default, userDto.Account);
        if (existUser != null)
        {
            var thirdPartyUser = new ThirdPartyUser(dto.ThirdPartyIdpId, existUser.Id, true, dto.ThridPartyIdentity, dto.ExtendedData);
            await _thirdPartyUserRepository.AddAsync(thirdPartyUser);
            return existUser.Adapt<UserModel>();
        }
        await _userDomainService.AddAsync(user);
        return user.Adapt<UserModel>();
    }

    public async Task<(ThirdPartyUser?, UserFriendlyException?)> VerifyRepeatAsync(Guid thirdPartyIdpId, string thridPartyIdentity)
    {
        var thirdPartyUser = await _authDbContext.Set<ThirdPartyUser>()
                                                 .Include(tpu => tpu.User)
                                                 .ThenInclude(user => user.Roles)
                                                 .FirstOrDefaultAsync(tpu => tpu.ThirdPartyIdpId == thirdPartyIdpId && tpu.ThridPartyIdentity == thridPartyIdentity);
        UserFriendlyException? exception = null;
        if (thirdPartyUser != null)
        {
            exception = new UserFriendlyException(UserFriendlyExceptionCodes.THIRD_PARTY_USER_EXIST, thridPartyIdentity);
        }
        return (thirdPartyUser, exception);
    }
}
