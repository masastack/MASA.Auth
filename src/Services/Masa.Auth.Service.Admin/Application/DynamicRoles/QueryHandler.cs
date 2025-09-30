// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles;

public class QueryHandler
{
    private readonly IDynamicRoleRepository _repository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IUserRepository _userRepository;
    private readonly DynamicRoleDomainService _dynamicRoleDomainService;

    public QueryHandler(
        IDynamicRoleRepository repository,
        II18n<DefaultResource> i18n,
        IUserRepository userRepository,
        DynamicRoleDomainService dynamicRoleDomainService)
    {
        _repository = repository;
        _i18n = i18n;
        _userRepository = userRepository;
        _dynamicRoleDomainService = dynamicRoleDomainService;
    }

    [EventHandler]
    public async Task GetAsync(DynamicRoleQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("DynamicRole"));

        var dto = entity.Adapt<DynamicRoleDto>();
        dto.SortConditions();

        query.Result = dto;
    }

    [EventHandler]
    public async Task GetPageAsync(DynamicRolePageQuery query)
    {
        var options = query.Options;
        var condition = await CreateFilteredPredicate(options);
        var pageList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(DynamicRole.ModificationTime)] = true,
            }
        });

        var creatorIds = pageList.Result.Select(x => x.Creator).Distinct().ToList();
        var modifierIds = pageList.Result.Select(x => x.Modifier).Distinct().ToList();
        var userIds = creatorIds.Union(modifierIds).ToList();

        var users = await _userRepository.AsQueryable().AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync();

        var dtos = pageList.Result.Select(x =>
        {
            var dto = x.Adapt<DynamicRoleDto>();
            var creator = users.FirstOrDefault(u => u.Id == x.Creator)?.DisplayName;
            var modifier = users.FirstOrDefault(u => u.Id == x.Modifier)?.DisplayName;
            dto.Creator = creator ?? string.Empty;
            dto.Modifier = modifier ?? string.Empty;
            dto.SortConditions();
            return dto;
        });

        var result = new PaginationDto<DynamicRoleDto>(pageList.Total, dtos.ToList());
        query.Result = result;
    }

    private async Task<Expression<Func<DynamicRole, bool>>> CreateFilteredPredicate(GetDynamicRoleInput options)
    {
        Expression<Func<DynamicRole, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(options.Search), x => x.Name.Contains(options.Search));
        condition = condition.And(!string.IsNullOrEmpty(options.ClientId), x => x.ClientId == options.ClientId);
        return await Task.FromResult(condition); ;
    }

    [EventHandler]
    public async Task UserRolesAsync(UserDynamicRoleQuery query)
    {
        if (query.RoleIds == null || !query.RoleIds.Any())
        {
            return;
        }

        var user = await _userRepository.AsQueryable()
            .Include(x => x.UserClaims)
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == query.UserId);
        MasaArgumentException.ThrowIfNull(user, _i18n.T(nameof(User)));

        var entitys = await _repository.GetListAsync(x => query.RoleIds.Contains(x.Id));

        var dtos = new List<DynamicRoleDto>();
        foreach (var entity in entitys)
        {
            var isValid = await _dynamicRoleDomainService.EvaluateConditionsAsync(user, entity);
            if (isValid)
            {
                dtos.Add(entity.Adapt<DynamicRoleDto>());
            }
        }

        query.Result = dtos;
    }
}
