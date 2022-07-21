// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Caching.Core.Models;

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly StaffDomainService _staffDomainService;
    readonly UserDomainService _userDomainService;
    readonly IUserContext _userContext;
    readonly IDistributedCacheClient _cache;
    readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;

    public CommandHandler(
        IUserRepository userRepository,
        IStaffRepository staffRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        StaffDomainService staffDomainService,
        UserDomainService userDomainService,
        IDistributedCacheClient cache,
        IUserContext userContext,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository)
    {
        _userRepository = userRepository;
        _staffRepository = staffRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _staffDomainService = staffDomainService;
        _userDomainService = userDomainService;
        _cache = cache;
        _userContext = userContext;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
    }

    #region User

    [EventHandler(1)]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        Expression<Func<User, bool>> condition = user => user.Account == userDto.Account;
        if (!string.IsNullOrEmpty(userDto.PhoneNumber))
            condition = condition.Or(user => user.PhoneNumber == userDto.PhoneNumber);
        if (!string.IsNullOrEmpty(userDto.Landline))
            condition = condition.Or(user => user.Landline == userDto.Landline);
        if (!string.IsNullOrEmpty(userDto.Email))
            condition = condition.Or(user => user.Email == userDto.Email);
        if (!string.IsNullOrEmpty(userDto.IdCard))
            condition = condition.Or(user => user.IdCard == userDto.IdCard);

        var user = await _userRepository.FindAsync(condition);
        if (user is not null)
        {
            if (string.IsNullOrEmpty(userDto.PhoneNumber) is false && userDto.PhoneNumber == user.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (string.IsNullOrEmpty(userDto.Landline) is false && userDto.Landline == user.Landline)
                throw new UserFriendlyException($"User with landline {userDto.Landline} already exists");
            if (string.IsNullOrEmpty(userDto.Account) is false && userDto.Account == user.Account)
                throw new UserFriendlyException($"User with account {userDto.Account} already exists");
            if (string.IsNullOrEmpty(userDto.Email) is false && userDto.Email == user.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
            if (string.IsNullOrEmpty(userDto.IdCard) is false && userDto.IdCard == user.IdCard)
                throw new UserFriendlyException($"User with idCard {userDto.IdCard} already exists");
        }
        else
        {
            user = new User(userDto.Name, userDto.DisplayName ?? "", userDto.Avatar ?? "", userDto.IdCard ?? "", userDto.Account ?? "", userDto.Password, userDto.CompanyName ?? "", userDto.Department ?? "", userDto.Position ?? "", userDto.Enabled, userDto.PhoneNumber ?? "", userDto.Landline, userDto.Email ?? "", userDto.Address, userDto.Gender);
            user.AddRoles(userDto.Roles.ToArray());
            user.AddPermissions(userDto.Permissions.Select(p => new UserPermission(p.PermissionId, p.Effect)).ToList());
            await _userRepository.AddAsync(user);
            command.NewUser = user;
            await _userDomainService.SetAsync(user);
        }
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserAsync(userDto.Id);

        Expression<Func<User, bool>> condition = user => false;
        if (!string.IsNullOrEmpty(userDto.PhoneNumber))
            condition = condition.Or(user => user.PhoneNumber == userDto.PhoneNumber);
        if (!string.IsNullOrEmpty(userDto.Landline))
            condition = condition.Or(user => user.Landline == userDto.Landline);
        if (!string.IsNullOrEmpty(userDto.Email))
            condition = condition.Or(user => user.Email == userDto.Email);
        if (!string.IsNullOrEmpty(userDto.IdCard))
            condition = condition.Or(user => user.IdCard == userDto.IdCard);

        Expression<Func<User, bool>> condition2 = user => user.Id != userDto.Id;
        var exitUser = await _userRepository.FindAsync(condition2.And(condition));
        if (exitUser is not null)
        {
            if (string.IsNullOrEmpty(userDto.PhoneNumber) is false && userDto.PhoneNumber == exitUser.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {userDto.PhoneNumber} already exists");
            if (string.IsNullOrEmpty(userDto.Landline) is false && userDto.Landline == exitUser.Landline)
                throw new UserFriendlyException($"User with landline {userDto.Landline} already exists");
            if (string.IsNullOrEmpty(userDto.Email) is false && userDto.Email == exitUser.Email)
                throw new UserFriendlyException($"User with email {userDto.Email} already exists");
            if (string.IsNullOrEmpty(userDto.IdCard) is false && userDto.IdCard == exitUser.IdCard)
                throw new UserFriendlyException($"User with idCard {userDto.IdCard} already exists");
        }
        else
        {
            user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
            await _userRepository.UpdateAsync(user);
            await _userDomainService.SetAsync(user);
        }
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        var user = await CheckUserAsync(command.User.Id);

        if (user.Account == "admin")
        {
            throw new UserFriendlyException("超级管理员 无法删除");
        }

        if (user.Id == _userContext.GetUserId<Guid>())
        {
            throw new UserFriendlyException("当前用户不能删除自己");
        }

        await _userRepository.RemoveAsync(user);
        await _userDomainService.RemoveAsync(user.Id);
    }

    [EventHandler(1)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.GetDetailAsync(userDto.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        user.AddRoles(userDto.Roles.ToArray());

        user.AddPermissions(userDto.Permissions.Select(p => new UserPermission(p.PermissionId, p.Effect)).ToList());
        await _userRepository.UpdateAsync(user);
        await _userDomainService.SetAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserPasswordAsync(ResetUserPasswordCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserAsync(userDto.Id);

        user.UpdatePassword(userDto.Password);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        //todo UserDomainService
        var key = $"auth_login_{validateByAccountCommand.Account}";
        var loginCache = await _cache.GetAsync<CacheLogin>(key);
        if (loginCache is not null && loginCache.LoginErrorCount >= 5) throw new UserFriendlyException("您连续输错密码5次,登录已冻结，请三十分钟后再次尝试");
        var user = await _userRepository.FindAsync(u => u.Account == validateByAccountCommand.Account);
        if (user != null)
        {
            if (!user.Enabled)
            {
                throw new UserFriendlyException("账号已禁用");
            }
            var success = user.VerifyPassword(validateByAccountCommand.Password);
            if (success)
            {
                if (loginCache is not null) await _cache.RemoveAsync<CacheLogin>(key);
            }
            else
            {
                loginCache ??= new() { FreezeTime = DateTimeOffset.Now.AddMinutes(30), Account = validateByAccountCommand.Account };
                loginCache.LoginErrorCount++;
                var options = new CombinedCacheEntryOptions<CacheLogin>
                {
                    DistributedCacheEntryOptions = new()
                    {
                        AbsoluteExpiration = loginCache.FreezeTime
                    }
                };
                await _cache.SetAsync(key, loginCache, options);
                throw new UserFriendlyException("账号或密码错误");
            }
            validateByAccountCommand.Result = success;
        }
    }

    [EventHandler]
    public async Task UpdateUserPasswordAsync(UpdateUserPasswordCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserAsync(userModel.Id);
        if (!user.VerifyPassword(userModel.OldPassword))
        {
            throw new UserFriendlyException("password verification failed");
        }
        user.UpdatePassword(userModel.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task UpdateUserBasicInfoAsync(UpdateUserBasicInfoCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserAsync(userModel.Id);
        user.UpdateBasicInfo(userModel.DisplayName, userModel.PhoneNumber, userModel.Email, userModel.Avatar, userModel.Gender);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.SetAsync(user);
    }

    private async Task<User> CheckUserAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(u => u.Id == userId);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        return user;
    }

    #endregion

    #region Staff

    [EventHandler]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await _staffDomainService.AddStaffAsync(command.Staff);
    }

    [EventHandler]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        await _staffDomainService.UpdateStaffAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task UpdateStaffPasswordAsync(UpdateStaffPasswordCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(u => u.Id == staffDto.Id);
        if (staff is null)
            throw new UserFriendlyException("The current user does not exist");

        staff.UpdatePassword(staffDto.Password);
        await _staffRepository.UpdateAsync(staff);
    }

    [EventHandler]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        var staff = await _staffRepository.FindAsync(command.Staff.Id);
        if (staff == null)
        {
            throw new UserFriendlyException("the current staff not found");
        }
        await _staffRepository.RemoveAsync(staff);
    }

    [EventHandler]
    public async Task SyncAsync(SyncStaffCommand command)
    {
        command.Result = await _staffDomainService.SyncStaffAsync(command.Staffs);
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler]
    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var exist = await _thirdPartyIdpRepository.GetCountAsync(thirdPartyIdp => thirdPartyIdp.Name == thirdPartyIdpDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ThirdPartyIdp with name {thirdPartyIdpDto.Name} already exists");

        var thirdPartyIdp = new ThirdPartyIdp(thirdPartyIdpDto.Name, thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, thirdPartyIdpDto.IdentificationType, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);

        await _thirdPartyIdpRepository.AddAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == thirdPartyIdpDto.Id);
        if (thirdPartyIdp is null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        thirdPartyIdp.Update(thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, thirdPartyIdpDto.IdentificationType, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);
        await _thirdPartyIdpRepository.UpdateAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task RemoveThirdPartyIdpAsync(RemoveThirdPartyIdpCommand command)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == command.ThirdPartyIdp.Id);
        if (thirdPartyIdp == null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        await _thirdPartyIdpRepository.RemoveAsync(thirdPartyIdp);
    }

    #endregion

    #region UserSystemData
    [EventHandler(1)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand command)
    {
        var data = command.UserSystemData;
        var item = await _userSystemBusinessDataRepository.FindAsync(userSystemBusinessData => userSystemBusinessData.UserId == data.UserId && userSystemBusinessData.SystemId == data.SystemId);
        if (item is null)
        {
            await _userSystemBusinessDataRepository.AddAsync(new UserSystemBusinessData(data.UserId, data.SystemId, data.Data));
        }
        else
        {
            item.Update(data.Data);
            await _userSystemBusinessDataRepository.UpdateAsync(item);
        }
    }
    #endregion
}
