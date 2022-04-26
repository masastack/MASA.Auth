namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ClientRepository : Repository<AuthDbContext, Client, int>, IClientRepository
{
    public ClientRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
