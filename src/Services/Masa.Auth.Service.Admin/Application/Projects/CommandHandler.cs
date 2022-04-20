using Masa.Auth.Service.Admin.Application.Projects.Commands;

namespace Masa.Auth.Service.Admin.Application.Projects;

public class CommandHandler
{
    readonly IAppNavigationTagRepository _appNavigationTagRepository;

    public CommandHandler(IAppNavigationTagRepository appNavigationTagRepository)
    {
        _appNavigationTagRepository = appNavigationTagRepository;
    }

    [EventHandler]
    public async Task SaveAppTagAysnc(UpsertAppTagCommand upsertAppTagCommand)
    {
        var appIdentity = upsertAppTagCommand.AppTagDetail.AppIdentity;
        var tag = upsertAppTagCommand.AppTagDetail.Tag;
        var item = await _appNavigationTagRepository.FindAsync(an => an.AppIdentity == appIdentity);
        if (item == null)
        {
            await _appNavigationTagRepository.AddAsync(new AppNavigationTag(appIdentity, tag));
        }
        else
        {
            item.UpdateTag(tag);
            await _appNavigationTagRepository.UpdateAsync(item);
        }
    }
}
