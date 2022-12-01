// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class LdapCommandHandler
{
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILdapFactory _ldapFactory;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly ILogger<LdapCommandHandler> _logger;
    readonly IConfiguration _configuration;
    readonly IEventBus _eventBus;

    public LdapCommandHandler(
        ILdapIdpRepository ldapIdpRepository,
        ILdapFactory ldapFactory,
        ILogger<LdapCommandHandler> logger,
        IThirdPartyUserRepository thirdPartyUserRepository,
        IConfiguration configuration,
        IEventBus eventBus)
    {
        _ldapIdpRepository = ldapIdpRepository;
        _ldapFactory = ldapFactory;
        _logger = logger;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _configuration = configuration;
        _eventBus = eventBus;
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
        var _thirdPartyIdpId = Guid.Empty;
        var ldapIdpDto = ldapUpsertCommand.LdapDetailDto;
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
            await _ldapIdpRepository.UnitOfWork.SaveChangesAsync();
            _thirdPartyIdpId = ldapIdp.Id;
        }
        else
        {
            _thirdPartyIdpId = dbItem.Id;
            dbItem.Update(ldapIdp);
            await _ldapIdpRepository.UpdateAsync(dbItem);
        }
        var ldapOptions = ldapIdpDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        var ldapUsers = ldapProvider.GetAllUserAsync();

        await foreach (var ldapUser in ldapUsers)
        {
            if (string.IsNullOrEmpty(ldapUser.Phone)) continue;
            try
            {
                var upsertThirdPartyUserCommand = new UpsertLdapUserCommand(ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser), ldapUser.Name, ldapUser.DisplayName, ldapUser.Phone, ldapUser.EmailAddress, ldapUser.SamAccountName, ldapUser.Phone);
                await _eventBus.PublishAsync(upsertThirdPartyUserCommand);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "LdapUser Name = {0},Email = {1},PhoneNumber={2}", ldapUser.Name, ldapUser.EmailAddress, ldapUser.Phone);
            }
        }
    }
}
