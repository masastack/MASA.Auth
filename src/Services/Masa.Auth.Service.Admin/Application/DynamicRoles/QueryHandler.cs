// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles;

public class QueryHandler
{
    private readonly IDynamicRoleRepository _repository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly OperaterProvider _operaterProvider;

    public QueryHandler(IDynamicRoleRepository repository, II18n<DefaultResource> i18n, OperaterProvider operaterProvider)
    {
        _repository = repository;
        _i18n = i18n;
        _operaterProvider = operaterProvider;
    }

    [EventHandler]
    public async Task GetAsync(DynamicRoleQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("DynamicRole"));

        query.Result = entity.Adapt<DynamicRoleDto>();
    }

    [EventHandler]
    public async Task GetPageAsync(DynamicRolePageQuery query)
    {
        var options = query.Options;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(DynamicRole.ModificationTime)] = true,
            }
        });

        var dtos = await Task.WhenAll(resultList.Result.Select(async x =>
        {
            var dto = x.Adapt<DynamicRoleDto>();
            var (creator, modifier) = await _operaterProvider.GetActionInfoAsync(x.Creator, x.Modifier);
            dto.Creator = creator;
            dto.Modifier = modifier;
            return dto;
        }));

        var result = new PaginationDto<DynamicRoleDto>(resultList.Total, dtos.ToList());
        query.Result = result;
    }

    private async Task<Expression<Func<DynamicRole, bool>>> CreateFilteredPredicate(GetDynamicRoleInput options)
    {
        Expression<Func<DynamicRole, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(options.Search), x => x.Name.Contains(options.Search));
        condition = condition.And(!string.IsNullOrEmpty(options.ClientId), x => x.ClientId == options.ClientId);
        return await Task.FromResult(condition); ;
    }
}
