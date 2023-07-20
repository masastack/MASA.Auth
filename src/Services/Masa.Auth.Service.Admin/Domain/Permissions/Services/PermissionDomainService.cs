// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Services;

public class PermissionDomainService : DomainService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionDomainService(
        IDomainEventBus eventBus,
        IPermissionRepository permissionRepository) : base(eventBus)
    {
        _permissionRepository = permissionRepository;
    }

    public bool CanAdd(Guid parentId, PermissionTypes permissionType)
    {
        if (parentId == Guid.Empty)
        {
            return true;
        }
        if (permissionType == PermissionTypes.Menu)
        {
            //menu and element can`t sibling node,element can`t menu parent
            return !_permissionRepository.Any(p => (p.ParentId == parentId && p.Type == PermissionTypes.Element)
                || (p.Id == parentId && (p.Type == PermissionTypes.Element || p.Type == PermissionTypes.Data)));
        }
        if (permissionType == PermissionTypes.Api)
        {
            return !_permissionRepository.Any(p => p.ParentId == parentId && p.Type != PermissionTypes.Api);
        }
        if (permissionType == PermissionTypes.Element)
        {
            return !_permissionRepository.Any(p => p.ParentId == parentId &&
                (p.Type == PermissionTypes.Menu || p.Type == PermissionTypes.Api));
        }
        if (permissionType == PermissionTypes.Data)
        {
            //todo
            return true;
        }
        return false;
    }
}
