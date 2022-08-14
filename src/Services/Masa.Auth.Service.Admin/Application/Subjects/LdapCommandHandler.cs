// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class LdapCommandHandler
{
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILdapFactory _ldapFactory;
    readonly ThirdPartyUserDomainService _thirdPartyUserDomainService;
    readonly IConfiguration _configuration;
    readonly ILogger<LdapCommandHandler> _logger;
    readonly IEventBus _eventBus;
    readonly AuthDbContext _authDbContext;

    public LdapCommandHandler(
        ILdapIdpRepository ldapIdpRepository,
        ILdapFactory ldapFactory,
        ThirdPartyUserDomainService thirdPartyUserDomainService,
        IMasaConfiguration masaConfiguration,
        ILogger<LdapCommandHandler> logger,
        IEventBus eventBus,
        AuthDbContext authDbContext)
    {
        _ldapIdpRepository = ldapIdpRepository;
        _ldapFactory = ldapFactory;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
        _configuration = masaConfiguration.Local;
        _logger = logger;
        _eventBus = eventBus;
        _authDbContext = authDbContext;
    }

    [EventHandler]
    public async Task LdapConnectTestAsync(LdapConnectTestCommand ldapConnectTestCommand)
    {
        var ldapOptions = ldapConnectTestCommand.LdapDetailDto.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
        if (!await ldapProvider.AuthenticateAsync(ldapOptions.RootUserDn, ldapOptions.RootUserPassword))
        {
            throw new UserFriendlyException("connect error");
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
            try
            {
                //todo:change bulk
                var thirdPartyUserDto = new UpsertThirdPartyUserDto(_thirdPartyIdpId, true, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser),
                    new AddUserDto
                    {
                        Name = ldapUser.Name,
                        DisplayName = ldapUser.DisplayName,
                        Enabled = true,
                        Email = ldapUser.EmailAddress,
                        Account = ldapUser.SamAccountName,
                        Password = DefaultUserAttributes.Password,
                        Avatar = DefaultUserAttributes.MaleAvatar
                    });
                //phone number regular match
                if (Regex.IsMatch(ldapUser.Phone, @"^1[3456789]\d{9}$"))
                {
                    thirdPartyUserDto.User.PhoneNumber = ldapUser.Phone;
                }
                else
                {
                    thirdPartyUserDto.User.Landline = ldapUser.Phone;
                }
                await _eventBus.PublishAsync(new UpsertThirdPartyUserCommand(thirdPartyUserDto));
                await _eventBus.PublishAsync(new UpsertStaffCommand(new UpsertStaffDto()));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "LdapUser Name = {0},Email = {1},PhoneNumber={2}", ldapUser.Name, ldapUser.EmailAddress, ldapUser.Phone);
            }
        }
    }
}
