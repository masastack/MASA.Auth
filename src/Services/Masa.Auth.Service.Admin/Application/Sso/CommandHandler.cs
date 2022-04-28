namespace Masa.Auth.Service.Admin.Application.Sso;

public class CommandHandler
{
    readonly ISsoClientRepository _ssoClientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;

    public CommandHandler(ISsoClientRepository ssoClientRepository, IIdentityResourceRepository identityResourceRepository)
    {
        _ssoClientRepository = ssoClientRepository;
        _identityResourceRepository = identityResourceRepository;
    }

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
        var idrs = await _identityResourceRepository.FindAsync(idrs => idrs.Id == command.IdentityResource.Id);
        if (idrs == null)
            throw new UserFriendlyException("The current identityResource does not exist");

        //Todo remove check
        await _identityResourceRepository.RemoveAsync(idrs);
    }

    #endregion
}
