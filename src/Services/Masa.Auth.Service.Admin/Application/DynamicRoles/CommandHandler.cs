// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles;

public class CommandHandler
{
    private readonly IDynamicRoleRepository _repository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public CommandHandler(IDynamicRoleRepository repository, II18n<DefaultResource> i18n, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IUserContext userContext)
    {
        _repository = repository;
        _i18n = i18n;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task CreateAsync(CreateDynamicRoleCommand command)
    {
        var entity = command.UpsertDto.Adapt<DynamicRole>();
        await _repository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateDynamicRoleCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("DynamicRole"));

        command.UpsertDto.Adapt(entity);

        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteDynamicRoleCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("DynamicRole"));

        await _repository.RemoveAsync(entity);
    }

    [EventHandler]
    public async Task ValidateAsync(ValidateDynamicRoleCommand command)
    {
        if (command.RoleIds == null || !command.RoleIds.Any())
        {
            command.Result = new();
            return;
        }

        var userId = _userContext.GetUserId<Guid>();
        var user = await _userRepository.AsQueryable().Include(x=>x.UserClaims).FirstOrDefaultAsync(x=>x.Id == userId);
        MasaArgumentException.ThrowIfNull(user, _i18n.T(nameof(User)));

        var entitys = await _repository.GetListAsync(x => command.RoleIds.Contains(x.Id));

        var dtos = entitys.Select(x=> new DynamicRoleValidateDto(x.Id, x.EvaluateConditions(user)));

        command.Result = dtos.ToList();
    }
}
