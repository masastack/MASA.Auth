// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class ThirdPartyQueryHandler
{
    private readonly AuthDbContext _authDbContext;
    private readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    private readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    private readonly ILdapIdpRepository _ldapIdpRepository;
    private readonly OperaterProvider _operaterProvider;

    public ThirdPartyQueryHandler(
        AuthDbContext authDbContext,
        IThirdPartyUserRepository thirdPartyUserRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        ILdapIdpRepository ldapIdpRepository,
        OperaterProvider operaterProvider)
    {
        _authDbContext = authDbContext;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _ldapIdpRepository = ldapIdpRepository;
        _operaterProvider = operaterProvider;
    }

    #region ThirdPartyUser

    [EventHandler]
    public async Task GetThirdPartyUsersAsync(ThirdPartyUsersQuery query)
    {
        Expression<Func<ThirdPartyUser, bool>> condition = tpu => true;
        condition = condition.And(query.Enabled is not null, tpu => tpu.Enabled == query.Enabled)
                             .And(query.StartTime is not null, tpu => tpu.CreationTime >= query.StartTime)
                             .And(query.EndTime is not null, tpu => tpu.CreationTime <= query.EndTime)
                             .And(query.ThirdPartyId is not null, tpu => tpu.ThirdPartyIdpId == query.ThirdPartyId)
                             .And(string.IsNullOrEmpty(query.Search) is false, tpu => tpu.User.DisplayName.Contains(query.Search!) || tpu.User.PhoneNumber.Contains(query.Search!) || tpu.User.Account.Contains(query.Search!));

        var tpuQuery = _authDbContext.Set<ThirdPartyUser>().Where(condition);
        var total = await tpuQuery.LongCountAsync();
        var tpus = await tpuQuery.Include(tpu => tpu.User)
                                 .AsNoTracking()
                                 .OrderByDescending(s => s.ModificationTime)
                                 .ThenByDescending(s => s.CreationTime)
                                 .Skip((query.Page - 1) * query.PageSize)
                                 .Take(query.PageSize)
                                 .ToListAsync();

        var resultList = new List<ThirdPartyUserDto>();
        foreach (var tpu in tpus)
        {
            var dto = (ThirdPartyUserDto)tpu;
            var (creator, modifier) = await _operaterProvider.GetActionInfoAsync(tpu.Creator, tpu.Modifier);
            dto.Creator = creator;
            dto.Modifier = modifier;
            resultList.Add(dto);
        }
        query.Result = new(total, resultList);
    }

    [EventHandler]
    public async Task GetThirdPartyUserDetailAsync(ThirdPartyUserDetailQuery query)
    {
        var tpu = await _thirdPartyUserRepository.GetDetail(query.ThirdPartyUserId);
        if (tpu is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.THIRD_PARTY_USER_NOT_FOUND);

        query.Result = tpu;

        var (creator, modifier) = await _operaterProvider.GetActionInfoAsync(tpu.Creator, tpu.Modifier);
        query.Result.User.Creator = creator;
        query.Result.User.Modifier = modifier;
    }

    [EventHandler]
    public async Task GetThirdPartyUserAsync(ThirdPartyUserQuery query)
    {
        var tpUser = await _authDbContext.Set<ThirdPartyUser>()
                                         .Include(tpu => tpu.User)
                                         .Include(tpu => tpu.User.Staff)
                                         .Include(tpu => tpu.User.Roles)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(tpu => tpu.ThridPartyIdentity == query.ThridPartyIdentity);
        var userModel = tpUser?.User?.Adapt<UserModel>();

        if (tpUser != null && tpUser.User != null && userModel != null)
        {
            var staff = tpUser.User.Staff;
            userModel.StaffId = (staff == null || !staff.Enabled) ? Guid.Empty : staff.Id;
            userModel.CurrentTeamId = staff?.CurrentTeamId;
            userModel.ClaimData = tpUser.ClaimData;
        }

        query.Result = userModel;
    }

    [EventHandler]
    public async Task GetThirdPartyUserByUserIdAsync(ThirdPartyUserByUserIdQuery query)
    {
        var tpUser = await _authDbContext.Set<ThirdPartyUser>()
                                         .Include(tpu => tpu.User)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(tpu => tpu.ThirdPartyIdpId == query.ThirdPartyIdpId && tpu.UserId == query.UserId);
        var userModel = tpUser?.User?.Adapt<UserModel>();

        if (tpUser != null && tpUser.User != null && userModel != null)
        {
            userModel.ClaimData = tpUser.ClaimData;
        }

        query.Result = userModel;
    }

    [EventHandler]
    public async Task GetThridPartyIdentityAsync(ThridPartyIdentityQuery query)
    {
        var tpUser = await _thirdPartyUserRepository.FindAsync(tpu => tpu.ThirdPartyIdpId == query.ThirdPartyIdpId && tpu.UserId == query.UserId);

        query.Result = tpUser?.ThridPartyIdentity;
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler]
    public async Task GetThirdPartyIdpsAsync(ThirdPartyIdpsQuery query)
    {
        Expression<Func<ThirdPartyIdp, bool>> condition = user => true;

        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(thirdPartyIdp => thirdPartyIdp.Name.Contains(query.Search) || thirdPartyIdp.DisplayName.Contains(query.Search));

        var thirdPartyIdps = await _thirdPartyIdpRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ThirdPartyIdp.ModificationTime)] = true,
                [nameof(ThirdPartyIdp.CreationTime)] = true,
            }
        });

        query.Result = new(thirdPartyIdps.Total, thirdPartyIdps.Result.Select(thirdPartyIdp => thirdPartyIdp.Adapt<ThirdPartyIdpDto>()).ToList());
    }

    [EventHandler]
    public async Task GetAllThirdPartyIdpAsync(AllThirdPartyIdpQuery query)
    {
        var thirdPartyIdps = await _thirdPartyIdpRepository.GetListAsync(tpIdp => tpIdp.Enabled);
        query.Result = thirdPartyIdps.Adapt<List<ThirdPartyIdpModel>>();
    }

    [EventHandler]
    public async Task GetThirdPartyIdpDetailAsync(ThirdPartyIdpDetailQuery query)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == query.ThirdPartyIdpId);
        if (thirdPartyIdp is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.THIRD_PARTY_IDP_NOT_FOUND);

        query.Result = thirdPartyIdp.Adapt<ThirdPartyIdpDetailDto>();
    }

    [EventHandler]
    public async Task GetIdentityProviderByTypeAsync(IdentityProviderByTypeQuery query)
    {
        var identityProvider = await _authDbContext.Set<IdentityProvider>()
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync(ip => ip.ThirdPartyIdpType == query.ThirdPartyIdpType);

        query.Result = identityProvider ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IDENTITY_PROVIDER_NOT_FOUND);
    }

    [EventHandler]
    public async Task GetIdentityProviderBySchemeAsync(IdentityProviderBySchemeQuery query)
    {
        var identityProvider = await _authDbContext.Set<IdentityProvider>()
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync(ip => ip.Name == query.scheme);

        query.Result = identityProvider ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IDENTITY_PROVIDER_NOT_FOUND);
    }

    [EventHandler]
    public async Task GetLdapDetailDtoAsync(LdapDetailQuery query)
    {
        var thirdPartyIdp = await _ldapIdpRepository.FindAsync(ldap => ldap.Name == query.Name);
        thirdPartyIdp?.Adapt(query.Result);
    }

    [EventHandler]
    public async Task GetThirdPartyIdpSelectAsync(ThirdPartyIdpSelectQuery query)
    {
        Expression<Func<ThirdPartyIdp, bool>> condition = ThirdPartyIdp => true;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(thirdPartyIdp => thirdPartyIdp.Name.Contains(query.Search) || thirdPartyIdp.DisplayName.Contains(query.Search));
        //Select 只用于下拉展示,不下发 ClientSecret(凭据仅经鉴权后的详情/配置接口返回)
        var thirdPartyIdps = await _thirdPartyIdpRepository.GetListAsync(condition.And(thirdPartyIdp => thirdPartyIdp.Enabled == true));
        query.Result = thirdPartyIdps.Select(tpIdp => new ThirdPartyIdpSelectDto(tpIdp.Id, tpIdp.Name, tpIdp.DisplayName, tpIdp.ClientId, tpIdp.Icon, tpIdp.AuthenticationType)).ToList();
        if (query.IncludeLdap)
        {
            var ldap = (await _ldapIdpRepository.GetListAsync()).FirstOrDefault();
            if (ldap is not null)
                query.Result.Add(new ThirdPartyIdpSelectDto(ldap.Id, ldap.Name, ldap.DisplayName, "", ldap.Icon, default));
        }
    }

    [EventHandler]
    public void GetExternalThirdPartyIdps(ExternalThirdPartyIdpsQuery query)
    {
        query.Result = LocalAuthenticationDefaultsProvider.GetAll()
                                                     .Adapt<List<ThirdPartyIdpModel>>()
                                                     .ToList();
    }

    #endregion

    [EventHandler]
    public async Task GetLdapUsersAccountAsync(LdapUsersAccountQuery query)
    {
        var identityProvider = await _authDbContext.Set<IdentityProvider>().FirstOrDefaultAsync(x => x.Name == "Ldap");
        MasaArgumentException.ThrowIfNull(identityProvider, nameof(IdentityProvider));

        var tpUsers = await _thirdPartyUserRepository.GetListAsync(x => x.ThirdPartyIdpId == identityProvider.Id && query.UserIds.Contains(x.UserId));

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        query.Result = tpUsers.ToDictionary(x => x.UserId, x =>
        {
            var ldapUser = JsonSerializer.Deserialize<LdapUser>(x.ExtendedData, options);
            return ldapUser?.SamAccountName ?? string.Empty;
        });
    }

    [EventHandler]
    public async Task GetThirdPartyUserFieldValueAsync(ThirdPartyUserFieldValueQuery query)
    {
        var identityProvider = await _authDbContext.Set<IdentityProvider>().FirstOrDefaultAsync(x => x.Name == query.Scheme);
        MasaArgumentException.ThrowIfNull(identityProvider, nameof(IdentityProvider));

        var tpUsers = await _thirdPartyUserRepository.GetListAsync(x => x.ThirdPartyIdpId == identityProvider.Id && query.UserIds.Contains(x.UserId));

        if (query.Field.IsNullOrEmpty())
        {
            query.Result = tpUsers.ToDictionary(x => x.UserId, x => x.ThridPartyIdentity);
            return;
        }

        query.Result = tpUsers.ToDictionary(x => x.UserId, x =>
        {
            var fieldValue = string.Empty;
            try
            {
                using var doc = JsonDocument.Parse(x.ExtendedData);
                var root = doc.RootElement;

                if (root.TryGetProperty(query.Field, out var fieldElement))
                {
                    fieldValue = fieldElement.GetString();
                }
            }
            catch (JsonException ex)
            {
                return string.Empty;
            }
            return fieldValue ?? string.Empty;
        });
    }
}
