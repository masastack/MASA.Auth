namespace Masa.Auth.Service.Admin.Application.Sso;

public class CommandHandler
{
    readonly IClientRepository _clientRepository;

    #region Client
    public CommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    [EventHandler]
    public async Task AddClientAsync(AddClientCommand addClientCommand)
    {
        var client = addClientCommand.ClientAddDto.Adapt<Client>();

        await _clientRepository.AddAsync(client);
    }

    [EventHandler]
    public async Task UpdateClientAsync(UpdateClientCommand updateClientCommand)
    {
        var id = updateClientCommand.ClientDetailDto.Id;
        var client = (await _clientRepository.FindAsync(id))
            ?? throw new UserFriendlyException($"Client id ={id} not found");
        updateClientCommand.ClientDetailDto.Adapt(client);

        await _clientRepository.UpdateAsync(client);
    }

    [EventHandler]
    public async Task RemoveClientAsync(RemoveClientCommand removeClientCommand)
    {
        var client = (await _clientRepository.FindAsync(removeClientCommand.ClientId))
            ?? throw new UserFriendlyException($"Client id ={removeClientCommand.ClientId} not found");
        await _clientRepository.RemoveAsync(client);
    }
    #endregion
}
