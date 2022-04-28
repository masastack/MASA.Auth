namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ClientDetailQuery(int ClientId) : Query<ClientDetailDto>
{
    public override ClientDetailDto Result { get; set; } = new();
}
