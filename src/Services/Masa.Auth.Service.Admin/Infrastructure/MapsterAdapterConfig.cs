namespace Masa.Auth.Service.Admin.Infrastructure;

public static class MapsterAdapterConfig
{
    public static void TypeAdapter()
    {
        TypeAdapterConfig<LdapDetailDto, LdapOptions>.ForType()
            .Map(dest => dest.ServerPort, src => src.IsLdaps ? 0 : src.ServerPort)
            .Map(dest => dest.ServerPortSsl, src => src.IsLdaps ? src.ServerPort : 0);
    }
}
