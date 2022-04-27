namespace Masa.Auth.Service.Admin.Infrastructure;

public class MapsterAdapterConfig
{
    public static void TypeAdapter()
    {
        TypeAdapterConfig<string, ClientGrantType>.NewConfig().MapWith(str => new ClientGrantType(str));
        TypeAdapterConfig<string, ClientRedirectUri>.NewConfig().MapWith(str => new ClientRedirectUri(str));
        TypeAdapterConfig<string, ClientPostLogoutRedirectUri>.NewConfig().MapWith(str => new ClientPostLogoutRedirectUri(str));
        TypeAdapterConfig<ClientGrantType, string>.NewConfig().MapWith(src => src.GrantType);
        TypeAdapterConfig<ClientRedirectUri, string>.NewConfig().MapWith(src => src.RedirectUri);
        TypeAdapterConfig<ClientPostLogoutRedirectUri, string>.NewConfig().MapWith(src => src.PostLogoutRedirectUri);
        //TypeAdapterConfig<ClientDetailDto, Client>.ForType().BeforeMapping((src, dest) =>
        //{
        //    dest.Properties.Clear();
        //    dest.RedirectUris.Clear();
        //    dest.PostLogoutRedirectUris.Clear();
        //});
        TypeAdapterConfig<ClientPropertyDto, ClientProperty>.NewConfig().MapToConstructor(true);
    }
}
