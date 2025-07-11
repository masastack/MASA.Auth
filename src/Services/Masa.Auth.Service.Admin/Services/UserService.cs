﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class UserService : ServiceBase
{
    public UserService() : base("api/user")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        RouteOptions.DisableAutoMapRoute = false;
        MapGet(GetListByRoleAsync, "getListByRole");
        MapGet(GetClaimValuesAsync, "claim-values/{id}");
        MapGet(GetClaimValuesAsync, "claim-values");
        MapPost(SaveClaimValuesAsync, "claim-values").RequireAuthorization();
        MapPost(SaveClaimValueAsync, "claim-value").RequireAuthorization();
    }

    public async Task<PaginationDto<UserDto>> GetListAsync(IEventBus eventBus, GetUsersDto user)
    {
        var query = new UsersQuery(user.Page, user.PageSize, user.UserId, user.Enabled, user.StartTime, user.EndTime);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<UserDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, Guid id)
    {
        var query = new UserDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<UserSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string search)
    {
        var query = new UserSelectQuery(search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<UserModel> AddExternalAsync(IEventBus eventBus, [FromBody] AddUserModel model)
    {
        var dto = new AddUserDto()
        {
            Account = model.Account,
            Name = model.Name,
            DisplayName = model.DisplayName,
            IdCard = model.IdCard,
            CompanyName = model.CompanyName,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Gender = model.Gender,
            Password = model.Password,
            Enabled = true,
        };
        var command = new AddUserCommand(dto);
        await eventBus.PublishAsync(command);
        return command.Result.Adapt<UserModel>();
    }

    [AllowAnonymous]
    [RoutePattern("upsertExternal", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<UserModel> UpsertExternalAsync(IEventBus eventBus, [FromBody] UpsertUserModel model)
    {
        var command = new UpsertUserCommand(model);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    public async Task<bool> PutDisableAsync(IEventBus eventBus, [FromBody] DisableUserModel model)
    {
        var command = new DisableUserCommand(model);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    public async Task<bool> GetVerifyRepeatAsync(IEventBus eventBus, VerifyUserRepeatDto user)
    {
        var query = new VerifyUserRepeatQuery(user);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task AddAsync(IEventBus eventBus, [FromBody] AddUserDto dto)
    {
        await eventBus.PublishAsync(new AddUserCommand(dto));
    }

    public async Task UpdateAsync(
        IEventBus eventBus,
        [FromBody] UpdateUserDto dto)
    {
        await eventBus.PublishAsync(new UpdateUserCommand(dto));
    }

    public async Task UpdateAuthorizationAsync(IEventBus eventBus,
        [FromBody] UpdateUserAuthorizationDto dto)
    {
        await eventBus.PublishAsync(new UpdateUserAuthorizationCommand(dto));
    }

    public async Task PutResetPasswordAsync(IEventBus eventBus,
        [FromBody] ResetUserPasswordDto dto)
    {
        await eventBus.PublishAsync(new ResetUserPasswordCommand(dto));
    }

    public async Task RemoveAsync(
        IEventBus eventBus,
        [FromBody] RemoveUserDto dto)
    {
        await eventBus.PublishAsync(new RemoveUserCommand(dto));
    }

    [AllowAnonymous]
    public async Task<UserModel?> PostValidateByAccountAsync(IEventBus eventBus, [FromBody] ValidateAccountModel validateAccountModel)
    {
        var validateCommand = new ValidateByAccountCommand(validateAccountModel);
        await eventBus.PublishAsync(validateCommand);
        return ConvertToModel(validateCommand.Result);
    }

    [AllowAnonymous]
    public async Task<UserModel?> FindByAccountAsync(IEventBus eventBus, [FromQuery] string account)
    {
        var query = new FindUserByAccountQuery(account);
        await eventBus.PublishAsync(query);
        return ConvertToModel(query.Result);
    }

    public async Task<List<UserSimpleModel>> GetListByAccountAsync(IEventBus eventBus, [FromQuery] string accounts)
    {
        var query = new UsersByAccountQuery(accounts.Split(','));
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    public async Task<UserModel?> FindByEmailAsync(IEventBus eventBus, [FromQuery] string email)
    {
        var query = new FindUserByEmailQuery(email);
        await eventBus.PublishAsync(query);
        return ConvertToModel(query.Result);
    }

    [AllowAnonymous]
    public async Task<UserModel?> FindByPhoneNumberAsync(IEventBus eventBus, [FromQuery] string phoneNumber)
    {
        var query = new FindUserByPhoneNumberQuery(phoneNumber);
        await eventBus.PublishAsync(query);
        return ConvertToModel(query.Result);
    }

    public async Task<UserModel> FindByIdAsync(IEventBus eventBus, Guid id)
    {
        var query = new UserDetailQuery(id);
        await eventBus.PublishAsync(query);
        return ConvertToModel(query.Result);
    }

    [return: NotNullIfNotNull("user")]
    private UserModel? ConvertToModel(UserDetailDto? user)
    {
        if (user == null) return null;
        return new UserModel()
        {
            Id = user.Id,
            Name = user.Name,
            Account = user.Account,
            DisplayName = user.DisplayName,
            StaffDisplayName = user.StaffDisplayName,
            IdCard = user.IdCard,
            CompanyName = user.CompanyName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Department = user.Department,
            Gender = user.Gender,
            Avatar = user.Avatar,
            CreationTime = user.CreationTime,
            Position = user.Position,
            Address = new AddressValueModel
            {
                Address = user.Address.Address,
                ProvinceCode = user.Address.ProvinceCode,
                CityCode = user.Address.CityCode,
                DistrictCode = user.Address.DistrictCode
            },
            Roles = user.Roles,
            StaffId = user.StaffId,
            CurrentTeamId = user.CurrentTeamId
        };
    }

    public async Task PostVisit(IEventBus eventBus, [FromBody] AddUserVisitedDto addUserVisitedDto)
    {
        var visitCommand = new UserVisitedCommand(addUserVisitedDto);
        await eventBus.PublishAsync(visitCommand);
    }

    public async Task<List<UserVisitedModel>> GetVisitedList(IEventBus eventBus, [FromQuery] Guid userId)
    {
        var visitListQuery = new UserVisitedListQuery(userId);
        await eventBus.PublishAsync(visitListQuery);
        return visitListQuery.Result;
    }

    public async Task UpdatePasswordAsync(IEventBus eventBus,
        [FromBody] UpdateUserPasswordModel user)
    {
        await eventBus.PublishAsync(new UpdateUserPasswordCommand(user));
    }

    public async Task UpdateBasicInfoAsync(IEventBus eventBus,
        [FromBody] UpdateUserBasicInfoModel user)
    {
        await eventBus.PublishAsync(new UpdateUserBasicInfoCommand(user));
    }

    public async Task UpdateAvatarAsync(IEventBus eventBus,
        [FromBody] UpdateUserAvatarModel staff)
    {
        await eventBus.PublishAsync(new UpdateUserAvatarCommand(staff));
    }

    public async Task<bool> UpdatePhoneNumberAsync(IEventBus eventBus,
        [FromBody] UpdateUserPhoneNumberModel model)
    {
        var command = new UpdateUserPhoneNumberCommand(model);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    [AllowAnonymous]
    public async Task<bool> PostVerifyMsgCodeAsync(IEventBus eventBus,
        [FromBody] VerifyMsgCodeModel model)
    {
        var command = new VerifyMsgCodeCommand(model);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    [AllowAnonymous]
    public async Task<UserModel?> PostLoginByPhoneNumberAsync(
        IEventBus eventBus,
        [FromBody] LoginByPhoneNumberModel model)
    {
        var command = new LoginByPhoneNumberCommand(model);
        await eventBus.PublishAsync(command);
        return ConvertToModel(command.Result); ;
    }

    public async Task RemoveUserRolesAsync(
        IEventBus eventBus,
        [FromBody] RemoveUserRolesModel model)
    {
        var command = new RemoveUserRolesCommand(model);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("byIds", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<List<UserModel>> PostPortraitsAsync(IEventBus eventBus,
        [FromBody] List<Guid> userIds)
    {
        var query = new UserPortraitsQuery(userIds);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task PostSystemData(IEventBus eventBus, [FromBody] UserSystemDataDto data)
    {
        var command = new SaveUserSystemBusinessDataCommand(data);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("systemData/byIds", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<Dictionary<string, string>> SystemListDataAsync(IEventBus eventBus, [FromBody] GetSystemDataModel model)
    {
        var query = new UserSystemBusinessDataQuery(model.UserIds, model.SystemId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    public async Task PostSyncAutoCompleteAsync(IEventBus eventBus, [FromBody] SyncUserAutoCompleteDto dto)
    {
        var command = new SyncUserAutoCompleteCommand(dto);
        await eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    [RoutePattern("SyncRedis", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task SyncRedisAsync(IEventBus eventBus)
    {
        var command = new SyncUserRedisCommand();
        await eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    [RoutePattern("register", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<UserModel> RegisterAsync(IEventBus eventBus, [FromBody] RegisterByEmailModel registerModel)
    {
        var command = new RegisterUserCommand(registerModel);
        await eventBus.PublishAsync(command);

        return command.Result;
    }

    [AllowAnonymous]
    public async Task<bool> GetHasPhoneNumberInEnvAsync(IEventBus eventBus, IMultiEnvironmentSetter environmentSetter,
        [FromQuery] string env, [FromQuery] string phoneNumber)
    {
        environmentSetter.SetEnvironment(env);
        var query = new UserByPhoneQuery(phoneNumber);
        await eventBus.PublishAsync(query);
        return query.Result is not null;
    }

    public async Task<bool> GetHasPasswordAsync(IEventBus eventBus, Guid userId)
    {
        var command = new HasPasswordQuery(userId);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    [AllowAnonymous]
    [RoutePattern("reset_password_by_phone", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<bool> ResetPasswordByPhoneAsync(IEventBus eventBus, [FromBody] ResetPasswordByPhoneModel model)
    {
        var command = new ResetPasswordCommand(ResetPasswordTypes.PhoneNumber, model.PhoneNumber, model.Code)
        {
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        };
        await eventBus.PublishAsync(command);
        return true;
    }

    [AllowAnonymous]
    [RoutePattern("reset_password_by_email", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<bool> ResetPasswordByEmailAsync(IEventBus eventBus, [FromBody] ResetPasswordByEmailModel model)
    {
        var command = new ResetPasswordCommand(ResetPasswordTypes.Email, model.Email, model.Code)
        {
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        };
        await eventBus.PublishAsync(command);
        return true;
    }

    public async Task<List<UserModel>> GetListByRoleAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new UsersByRoleQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    public async Task PostLoginByAccountAsync(IEventBus eventBus, [FromBody] LoginByAccountCommand command)
    {
        await eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    [RoutePattern("bind_roles", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task BindRolesAsync(IEventBus eventBus, [FromBody] BindUserRolesModel model)
    {
        var command = new BindUserRolesCommand(model);
        await eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    [RoutePattern("unbind_roles", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task UnbindRolesAsync(IEventBus eventBus, [FromBody] UnbindUserRolesModel model)
    {
        var command = new UnbindUserRolesCommand(model);
        await eventBus.PublishAsync(command);
    }

    public async Task<List<ClaimValue>> GetClaimValuesAsync(IEventBus eventBus, Guid Id)
    {
        var query = new UserClaimValuesQuery(Id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task SaveClaimValuesAsync(IEventBus eventBus, UserClaimValuesDto userClaimValues)
    {
        var command = new SaveUserClaimValuesCommand(userClaimValues.UserId, userClaimValues.ClaimValues);
        await eventBus.PublishAsync(command);
    }

    public async Task SaveClaimValueAsync(IEventBus eventBus, SaveClaimValueInput input)
    {
        var command = new SaveUserClaimValueCommand(input.UserId, input.ClaimName, input.ClaimValue);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("impersonate", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<ImpersonateOutput> ImpersonateAsync(IEventBus eventBus, [FromBody] ImpersonateInput input)
    {
        var command = new ImpersonateUserCommand(input.UserId, false);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    [AllowAnonymous]
    [RoutePattern("impersonate", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<ImpersonationCacheItem> GetImpersonatedAsync([FromServices] IEventBus eventBus, [FromQuery] string impersonationToken)
    {
        var query = new ImpersonatedUserQuery(impersonationToken);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("account/delete", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task DeleteAccountAsync([FromServices] IEventBus eventBus, [FromBody] DeleteAccountModel model)
    {
        var command = new DeleteAccountCommand(model.SmsCode);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("generate-password", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<string> GenerateNewPasswordAsync([FromServices] IEventBus eventBus)
    {
        var command = new GenerateNewPasswordCommand();
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    [RoutePattern("has-role", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<bool> HasRoleAsync(IEventBus eventBus, IUserContext _userContext, [FromBody] List<Guid> RoleIds)
    {
        var query = new UserHasAnyRoleQuery(_userContext.GetUserId<Guid>(), RoleIds);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
