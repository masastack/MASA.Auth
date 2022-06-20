// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class ThirdPartyUserDomainService : DomainService
{
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly IUserRepository _userRepository;
    readonly ILogger<ThirdPartyUserDomainService> _logger;

    public ThirdPartyUserDomainService(IDomainEventBus eventBus, IThirdPartyUserRepository thirdPartyUserRepository,
        IUserRepository userRepository, ILogger<ThirdPartyUserDomainService> logger)
        : base(eventBus)
    {
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task AddThirdPartyUserAsync(AddThirdPartyUserDto thirdPartyUserDto)
    {
#warning change _userRepository、_thirdPartyUserRepository to _cacheClient
        var user = await _userRepository.FindAsync(u => u.Account == thirdPartyUserDto.User.Account &&
                    u.Email == thirdPartyUserDto.User.Email &&
                    u.PhoneNumber == thirdPartyUserDto.User.PhoneNumber && u.Landline == thirdPartyUserDto.User.Landline);
        if (user != null)
        {
            var updateUserDto = thirdPartyUserDto.User.Adapt<UpdateUserDto>();
            updateUserDto.Id = user.Id;
            var updateUserCommand = new UpdateUserCommand(updateUserDto);
            await EventBus.PublishAsync(updateUserCommand);
        }
        else
        {
            var addUserCommand = new AddUserCommand(thirdPartyUserDto.User);
            await EventBus.PublishAsync(addUserCommand);
            await _thirdPartyUserRepository.AddAsync(new ThirdPartyUser(thirdPartyUserDto.ThirdPartyIdpId,
                        addUserCommand.NewUser.Id, thirdPartyUserDto.Enabled, thirdPartyUserDto.ThridPartyIdentity));
        }
    }
}
