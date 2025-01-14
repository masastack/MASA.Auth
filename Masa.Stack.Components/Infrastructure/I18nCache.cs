namespace Masa.Stack.Components.Infrastructure;

public class I18nCache : IScopedDependency
{
    public Dictionary<string, string> Section { get; private set; } = new();
    private readonly I18n _i18N;
    private readonly IDccClient _dccClient;
    public event Action? OnSectionUpdated;

    public I18nCache(
        IClientScopeServiceProviderAccessor serviceProviderAccessor, IDccClient dccClient)
    {
        _i18N = serviceProviderAccessor.ServiceProvider.GetRequiredService<I18n>();
        _dccClient = dccClient;

        if (_i18N != null)
        {
            _i18N.CultureChanged += async (object? sender, EventArgs arg) =>
            {
                await InitializeAsync();
            };
        }
    }

    public virtual async Task InitializeAsync()
    {
        var culture = _i18N.Culture.Name;
        try
        {
            Section = await _dccClient.OpenApiService.GetI18NConfigAsync(culture);
        }
        catch (Exception)
        {
            Section = new();
        }

        OnSectionUpdated?.Invoke();
    }
}

