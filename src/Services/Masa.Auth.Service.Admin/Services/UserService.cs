// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Caching;
using StackExchange.Redis;
using System.IO.Compression;

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
        MapPost(SaveClaimValuesAsync, "claim-values");
        MapPost(SaveClaimValueAsync, "claim-value");
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

    public async Task<Dictionary<string, string>> GetClaimValuesAsync(IEventBus eventBus, Guid Id)
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

    [AllowAnonymous]
    public async Task<UserModel?> GetUserAsync([FromServices] IDistributedCacheClient cacheClient, Guid userId)
    {
        var userModel = await cacheClient.GetAsync<UserModel>(CacheKey.UserKey(userId));
        return userModel;
    }

    [AllowAnonymous]
    public async Task<string> GetUser2Async([FromServices] ConnectionMultiplexer connect, Guid userId)
    {
        var _redis = connect.GetDatabase();
        var json = await _redis.HashGetAsync("UserModel." + CacheKey.UserKey(userId), "data");
        return json;
    }

    [AllowAnonymous]
    public async Task<UserModel?> GetUser3Async([FromServices] ConnectionMultiplexer connect, Guid userId)
    {
        var GlobalJsonSerializerOptions = new JsonSerializerOptions().EnableDynamicTypes();
        var _redis = connect.GetDatabase();
        var key = "UserModel." + CacheKey.UserKey(userId);
        var redisValues = await _redis.HashGetAsync(key, RedisConstant.ABSOLUTE_EXPIRATION_KEY, RedisConstant.SLIDING_EXPIRATION_KEY,
            RedisConstant.DATA_KEY).ConfigureAwait(false);
        var dataCacheInfo = RedisHelper.ConvertToCacheModel<UserModel>(key, redisValues, GlobalJsonSerializerOptions);

        //await dataCacheInfo.TrySetValueAsync(() => Refresh(_redis, dataCacheInfo, CommandFlags.None), null).ConfigureAwait(false);

        return dataCacheInfo.Value;
    }

    [AllowAnonymous]
    public async Task<UserModel?> GetUser4Async([FromServices] ConnectionMultiplexer connect, Guid userId)
    {
        var GlobalJsonSerializerOptions = new JsonSerializerOptions().EnableDynamicTypes();
        var _redis = connect.GetDatabase();
        var key = "UserModel." + CacheKey.UserKey(userId);
        var redisValues = await _redis.HashGetAsync(key, RedisConstant.ABSOLUTE_EXPIRATION_KEY, RedisConstant.SLIDING_EXPIRATION_KEY,
            RedisConstant.DATA_KEY).ConfigureAwait(false);
        var dataCacheInfo = RedisHelper.ConvertToCacheModel<UserModel>(key, redisValues, GlobalJsonSerializerOptions);

        await dataCacheInfo.TrySetValueAsync(() => Refresh(_redis, dataCacheInfo, CommandFlags.None), null).ConfigureAwait(false);

        return dataCacheInfo.Value;
    }

    private void Refresh(IDatabase Db,DataCacheModel model, CommandFlags flags)
    {
        var result = model.GetExpiration();
        if (result.State) Db.KeyExpire(model.Key, result.Expire, flags);
    }
}

internal static class DataCacheOptionsExtensions
{
    internal static (bool State, TimeSpan? Expire) GetExpiration(
        this DataCacheBaseModel model,
        DateTimeOffset? createTime = null,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (model.SlidingExpiration.HasValue)
        {
            TimeSpan? expr;
            if (model.AbsoluteExpiration.HasValue)
            {
                var sldExpr = GetSlidingExpiration(model.SlidingExpiration);
                var absExpr = new DateTimeOffset(model.AbsoluteExpiration.Value, TimeSpan.Zero);

                var relExpr = absExpr - (createTime ?? DateTimeOffset.Now);
                expr = relExpr <= sldExpr ? relExpr : sldExpr;
            }
            else
            {
                expr = GetSlidingExpiration(model.SlidingExpiration);
            }

            return (true, expr);
        }

        return (false, null);
    }

    private static TimeSpan GetSlidingExpiration(long? slidingExpiration) => new(slidingExpiration!.Value);
}


internal static class DatabaseExtensions
{
    public static RedisValue[] HashGet(
        this IDatabase database,
        string key,
        params string[] hashFields)
        => database.HashGet(key, hashFields.ToRedisValueArray());

    public static Task<RedisValue[]> HashGetAsync(
        this IDatabase database,
        string key,
        params string[] hashFields)
        => database.HashGetAsync(key, hashFields.ToRedisValueArray());
}


internal class DataCacheModel<T> : DataCacheModel
{
    public T? Value { get; private set; }

    public DataCacheModel(
        string key,
        long? absoluteExpiration,
        long? slidingExpiration,
        RedisValue redisValue,
        JsonSerializerOptions jsonSerializerOptions)
        : base(key, absoluteExpiration, slidingExpiration, redisValue, jsonSerializerOptions)
    {
        if (IsExist) Value = RedisValue.DecompressToValue<T>(JsonSerializerOptions);
    }

    public void TrySetValue(Action existAction, Func<T>? notExistFunc)
    {
        if (IsExist) existAction.Invoke();

        else if (notExistFunc != null) Value = notExistFunc.Invoke();
    }

    public async Task TrySetValueAsync(Action existAction, Func<Task<T>>? notExistFunc)
    {
        if (IsExist) existAction.Invoke();

        else if (notExistFunc != null) Value = await notExistFunc.Invoke().ConfigureAwait(false);
    }
}

/// <summary>
/// Data stored to Redis
/// </summary>
internal class DataCacheModel : DataCacheBaseModel
{
    public RedisValue RedisValue { get; }

    protected JsonSerializerOptions JsonSerializerOptions { get; }

    public bool IsExist { get; }

    public DataCacheModel(
        string key,
        long? absoluteExpiration,
        long? slidingExpiration,
        RedisValue redisValue,
        JsonSerializerOptions jsonSerializerOptions) : base(key, absoluteExpiration, slidingExpiration)
    {
        RedisValue = redisValue;
        JsonSerializerOptions = jsonSerializerOptions;
        IsExist = redisValue is { HasValue: true, IsNullOrEmpty: false };
    }
}


internal class DataCacheBaseModel
{
    public string Key { get; }

    public long? AbsoluteExpiration { get; }

    public long? SlidingExpiration { get; }

    public DataCacheBaseModel(string key, long? absoluteExpiration, long? slidingExpiration)
    {
        Key = key;
        AbsoluteExpiration = absoluteExpiration;
        SlidingExpiration = slidingExpiration;
    }
}

internal static class RedisValueExtensions
{
    public static HashEntry[] ConvertToHashEntries(
        this RedisValue redisValue,
        CacheExpiredModel cacheExpiredModel)
    {
        var hashEntries = new List<HashEntry>()
        {
            new("absexp", cacheExpiredModel.AbsoluteExpirationTicks),
            new("sldexp", cacheExpiredModel.SlidingExpirationTicks),
            new HashEntry("data", redisValue)
        };

        return hashEntries.ToArray();
    }

    public static T? DecompressToValue<T>(this RedisValue redisValue, JsonSerializerOptions jsonSerializerOptions)
    {
        var type = typeof(T);
        var compressMode = RedisHelper.GetCompressMode(type, out Type actualType);

        if (compressMode == CompressMode.None)
            return (T?)Convert.ChangeType(redisValue, actualType);

        var byteValue = (byte[])redisValue;
        if (byteValue.Length == 0)
            return default;

        var value = Decompress(byteValue);

        if (compressMode == CompressMode.Compress)
        {
            var valueString = Encoding.UTF8.GetString(value);
            return (dynamic)valueString;
        }

        return JsonSerializer.Deserialize<T>(value, jsonSerializerOptions);
    }

    private static byte[] Decompress(byte[] data)
    {
        using MemoryStream ms = new MemoryStream(data);
        using GZipStream stream = new GZipStream(ms, CompressionMode.Decompress);
        using MemoryStream outBuffer = new MemoryStream();
        byte[] block = new byte[1024];
        while (true)
        {
            int bytesRead = stream.Read(block, 0, block.Length);
            if (bytesRead <= 0)
                break;
            else
                outBuffer.Write(block, 0, bytesRead);
        }
        return outBuffer.ToArray();

    }
}

internal class CacheExpiredModel
{
    public long AbsoluteExpirationTicks { get; set; }

    public long SlidingExpirationTicks { get; set; }

    public long Expired { get; set; }

    public CacheExpiredModel(long absoluteExpirationTicks, long slidingExpirationTicks, long expired)
    {
        AbsoluteExpirationTicks = absoluteExpirationTicks;
        SlidingExpirationTicks = slidingExpirationTicks;
        Expired = expired;
    }
}

internal static class RedisHelper
{
    internal static CompressMode GetCompressMode(Type type, out Type actualType)
    {
        actualType = Nullable.GetUnderlyingType(type) ?? type;

        switch (Type.GetTypeCode(actualType))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Double:
            case TypeCode.Single:
            case TypeCode.Decimal:
                return CompressMode.None;
            case TypeCode.String:
                return CompressMode.Compress;
            default:
                return CompressMode.SerializeAndCompress;
        }
    }

    public static RedisKey[] GetRedisKeys(this IEnumerable<string> keys)
        => keys.Select(key => (RedisKey)key).ToArray();

    public static DataCacheBaseModel ConvertToCacheBaseModel(string key, RedisValue[] values)
    {
        var absoluteExpirationTicks = (long?)values[0];
        if (absoluteExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            absoluteExpirationTicks = null;

        var slidingExpirationTicks = (long?)values[1];
        if (slidingExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            slidingExpirationTicks = null;
        return new DataCacheBaseModel(key, absoluteExpirationTicks, slidingExpirationTicks);
    }

    public static DataCacheModel<T> ConvertToCacheModel<T>(
        string key,
        RedisValue[] values,
        JsonSerializerOptions jsonSerializerOptions)
    {
        RedisValue data = values.Length > 2 ? values[2] : RedisValue.Null;
        var absoluteExpirationTicks = (long?)values[0];
        if (absoluteExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            absoluteExpirationTicks = null;

        var slidingExpirationTicks = (long?)values[1];
        if (slidingExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            slidingExpirationTicks = null;
        return new DataCacheModel<T>(key, absoluteExpirationTicks, slidingExpirationTicks, data, jsonSerializerOptions);
    }

    public static DataCacheModel<T> ConvertToCacheModel<T>(
        string key,
        HashEntry[] hashEntries,
        JsonSerializerOptions jsonSerializerOptions)
    {
        var item = FormatHashEntries(hashEntries);
        return new DataCacheModel<T>(
            key,
            item.AbsoluteExpirationTicks,
            item.SlidingExpirationTicks,
            item.RedisValue,
            jsonSerializerOptions);
    }

    private static (long? AbsoluteExpirationTicks, long? SlidingExpirationTicks, RedisValue RedisValue) FormatHashEntries(
        HashEntry[] hashEntries)
    {
        long? absoluteExpiration = null;
        long? slidingExpiration = null;
        RedisValue data = RedisValue.Null;
        foreach (var hashEntry in hashEntries)
        {
            if (hashEntry.Name == RedisConstant.ABSOLUTE_EXPIRATION_KEY)
            {
                if (hashEntry.Value.HasValue && hashEntry.Value != RedisConstant.DEADLINE_LASTING)
                {
                    absoluteExpiration = (long?)hashEntry.Value;
                }
            }
            else if (hashEntry.Name == RedisConstant.SLIDING_EXPIRATION_KEY)
            {
                if (hashEntry.Value.HasValue && hashEntry.Value != RedisConstant.DEADLINE_LASTING)
                {
                    slidingExpiration = (long?)hashEntry.Value;
                }
            }
            else if (hashEntry.Name == RedisConstant.DATA_KEY)
            {
                data = hashEntry.Value;
            }
        }
        return new(absoluteExpiration, slidingExpiration, data);
    }
}

internal static class RedisConstant
{
    public const string DEFAULT_REDIS_SECTION_NAME = "RedisConfig";

    public const string DEFAULT_REDIS_HOST = "localhost";

    public const int DEFAULT_REDIS_PORT = 6379;

    public const string ABSOLUTE_EXPIRATION_KEY = "absexp";

    public const string SLIDING_EXPIRATION_KEY = "sldexp";

    public const string DATA_KEY = "data";

    public const int DEADLINE_LASTING = -1;

    public const string SET_MULTIPLE_SCRIPT = @"
                local count = 0
                for i, key in ipairs(KEYS) do
                  redis.call('HSET', key, '" + ABSOLUTE_EXPIRATION_KEY + "', ARGV[1], '" + SLIDING_EXPIRATION_KEY + @"', ARGV[2], '" +
        DATA_KEY + @"', ARGV[i+3])
                  if ARGV[3] ~= '-1' then
                    redis.call('EXPIRE', key, ARGV[3])
                  end
                  count = count + 1
                end
                return count";

    public const string SET_EXPIRE_SCRIPT = @"
        for index,key in ipairs(KEYS) do redis.call('expire', key, ARGV[index]) end;
        return 1";

    public const string GET_KEYS_SCRIPT = @"return redis.call('keys', @pattern)";

    public const string GET_KEY_AND_VALUE_SCRIPT = @"local ks = redis.call('KEYS', @keypattern)
        local result = {}
        for index,val in pairs(ks) do result[(2 * index - 1)] = val; result[(2 * index)] = redis.call('hgetall', val) end;
        return result";

    // KEYS[1] = key
    // ARGV[1] = absolute-expiration - ticks as long (-1 for none)
    // ARGV[2] = sliding-expiration - ticks as long (-1 for none)
    // ARGV[3] = relative-expiration (long, in seconds, -1 for none) - Min(absolute-expiration - Now, sliding-expiration)
    // this order should not change LUA script depends on it
    public const string SET_EXPIRATION_SCRIPT = @"
                local exist = redis.call('EXISTS',KEYS[1])
                if(exist ~= 1) then
                  return 0 end
                redis.call('HSET', KEYS[1], '" + ABSOLUTE_EXPIRATION_KEY + "', ARGV[1], '" + SLIDING_EXPIRATION_KEY + @"', ARGV[2])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                else
                  redis.call('PERSIST', KEYS[1])
                end
                return 1";

    public const string SET_MULTIPLE_EXPIRATION_SCRIPT = @"
                local count = 0
                for i, key in ipairs(KEYS) do
                  if(redis.call('EXISTS', key) == 1) then
                    redis.call('HSET', key, '" + ABSOLUTE_EXPIRATION_KEY + "', ARGV[1], '" + SLIDING_EXPIRATION_KEY + @"', ARGV[2])
                    if ARGV[3] ~= '-1' then
                      redis.call('EXPIRE', key, ARGV[3])
                    else
                      redis.call('PERSIST', key)
                    end
                    count = count + 1
                  end
                end
                return count";
}
internal enum CompressMode
{
    None = 1,
    Compress,
    SerializeAndCompress
}