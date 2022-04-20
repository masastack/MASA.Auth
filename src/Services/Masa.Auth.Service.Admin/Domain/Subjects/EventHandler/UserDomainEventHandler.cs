namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class UserDomainEventHandler
{
    readonly IAutoCompleteClient _autoCompleteClient;

    public UserDomainEventHandler(IAutoCompleteClient autoCompleteClient)
    {
        _autoCompleteClient = autoCompleteClient;
    }

    [EventHandler]
    public async Task SetUserAsync(SetUserDomainEvent userEvent)
    {
        var user = userEvent.user;
        var response = await _autoCompleteClient.SetAsync<UserSelectDto, Guid>(new UserSelectDto[]
        {
            new (user.Id,user.Name,user.Account,user.PhoneNumber,user.Email,user.Avatar)
        });
    }

    [EventHandler]
    public async Task RemoveUserAsync(RemoveUserDomainEvent userEvent)
    {
        await Task.CompletedTask;
    }
}
