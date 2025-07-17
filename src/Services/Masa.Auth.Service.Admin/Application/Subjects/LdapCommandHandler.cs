// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class LdapCommandHandler
{
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILdapFactory _ldapFactory;
    readonly ILogger<LdapCommandHandler> _logger;
    readonly IEventBus _eventBus;
    readonly IUnitOfWork _unitOfWork;
    readonly LdapDomainService _ldapDomainService;

    public LdapCommandHandler(
        ILdapIdpRepository ldapIdpRepository,
        ILdapFactory ldapFactory,
        ILogger<LdapCommandHandler> logger,
        IEventBus eventBus,
        IUnitOfWork unitOfWork,
        LdapDomainService ldapDomainService)
    {
        _ldapIdpRepository = ldapIdpRepository;
        _ldapFactory = ldapFactory;
        _logger = logger;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _ldapDomainService = ldapDomainService;
    }

    [EventHandler]
    public async Task LdapConnectTestAsync(LdapConnectTestCommand ldapConnectTestCommand)
    {
        var ldapOptions = ldapConnectTestCommand.LdapDetailDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        if (!await ldapProvider.AuthenticateAsync(ldapOptions.RootUserDn, ldapOptions.RootUserPassword))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CONNECT_ERROR);
        }
    }

    [EventHandler]
    public async Task LdapUpsertAsync(LdapUpsertCommand ldapUpsertCommand)
    {
        var ldapIdpDto = ldapUpsertCommand.LdapDetailDto;

        var ldapOptions = ldapIdpDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        if (!await ldapProvider.AuthenticateAsync(ldapOptions.RootUserDn, ldapOptions.RootUserPassword))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CONNECT_ERROR);
        }

        var ldapIdp = new LdapIdp(
                ldapIdpDto.ServerAddress,
                ldapIdpDto.ServerPort,
                ldapIdpDto.IsLdaps,
                ldapIdpDto.BaseDn,
                ldapIdpDto.RootUserDn,
                ldapIdpDto.RootUserPassword,
                ldapIdpDto.UserSearchBaseDn,
                ldapIdpDto.GroupSearchBaseDn);
        var dbItem = await _ldapIdpRepository.FindAsync(l => l.Name == ldapIdp.Name);
        if (dbItem is null)
        {
            await _ldapIdpRepository.AddAsync(ldapIdp);
        }
        else
        {
            dbItem.Update(ldapIdp);
            await _ldapIdpRepository.UpdateAsync(dbItem);
        }

        await _unitOfWork.SaveChangesAsync();

        var ldapUsers = new List<LdapUser>();
        await foreach (var user in ldapProvider.GetAllUserAsync())
        {
            ldapUsers.Add(user);
        }

        var args = new SyncLdapUserArgs()
        {
            LdapUsers = ldapUsers
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}
