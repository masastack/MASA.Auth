// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso;

public class CommandHandler
{
    readonly IClientRepository _clientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;

    public CommandHandler(IClientRepository clientRepository, IIdentityResourceRepository identityResourceRepository)
    {
        _clientRepository = clientRepository;
        _identityResourceRepository = identityResourceRepository;
    }

    #region Client
    [EventHandler]
    public async Task AddClientAsync(AddClientCommand addClientCommand)
    {
        PrepareGrantTypeWithClientType(addClientCommand.AddClientDto);
        var client = addClientCommand.AddClientDto.Adapt<Client>();

        await _clientRepository.AddAsync(client);

        void PrepareGrantTypeWithClientType(AddClientDto client)
        {
            switch (client.ClientType)
            {
                case ClientTypes.Web:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = true;
                    break;
                case ClientTypes.Spa:
                case ClientTypes.Native:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
                    break;
                case ClientTypes.Machine:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.ClientCredentials);
                    break;
                case ClientTypes.Device:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.DeviceFlow);
                    client.RequireClientSecret = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [EventHandler]
    public async Task UpdateClientAsync(UpdateClientCommand updateClientCommand)
    {
        var id = updateClientCommand.ClientDetailDto.Id;
        var client = await _clientRepository.GetByIdAsync(id);
        //Contrary to DDD
        updateClientCommand.ClientDetailDto.Adapt(client);
        await _clientRepository.UpdateAsync(client);
    }

    [EventHandler]
    public async Task RemoveClientAsync(RemoveClientCommand removeClientCommand)
    {
        var client = (await _clientRepository.FindAsync(removeClientCommand.ClientId))
            ?? throw new UserFriendlyException($"Client id = {removeClientCommand.ClientId} not found");
        await _clientRepository.RemoveAsync(client);
    }
    #endregion

    #region IdentityResource

    [EventHandler]
    public async Task AddIdentityResourceAsync(AddIdentityResourceCommand command)
    {
        var idrsDto = command.IdentityResource;
        var exist = await _identityResourceRepository.GetCountAsync(idrs => idrs.Name == idrsDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"IdentityResource with name {idrsDto.Name} already exists");

        var idrs = new IdentityResource(idrsDto.Name, idrsDto.DisplayName, idrsDto.Description, idrsDto.Enabled, idrsDto.Required, idrsDto.Emphasize, idrsDto.ShowInDiscoveryDocument, idrsDto.NonEditable);
        idrs.BindUserClaims(idrsDto.UserClaims);
        idrs.BindProperties(idrsDto.Properties);

        await _identityResourceRepository.AddAsync(idrs);
    }

    [EventHandler]
    public async Task UpdateIdentityResourceAsync(UpdateIdentityResourceCommand command)
    {
        var idrsDto = command.IdentityResource;
        var idrs = await _identityResourceRepository.FindAsync(idrs => idrs.Id == idrsDto.Id);
        if (idrs is null)
            throw new UserFriendlyException("The current identityResource does not exist");

        idrs.Update(idrsDto.DisplayName, idrsDto.Description, idrsDto.Enabled, idrsDto.Required, idrsDto.Emphasize, idrsDto.ShowInDiscoveryDocument, idrsDto.NonEditable);
        await _identityResourceRepository.UpdateAsync(idrs);
    }

    [EventHandler]
    public async Task RemoveIdentityResourceAsync(RemoveIdentityResourceCommand command)
    {
        var idrs = await _identityResourceRepository.FindAsync(idrs => idrs.Id == command.identityResource.Id);
        if (idrs == null)
            throw new UserFriendlyException("The current identityResource does not exist");

        //Todo remove check
        await _identityResourceRepository.RemoveAsync(idrs);
    }

    #endregion
}
