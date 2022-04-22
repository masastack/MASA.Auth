namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class LDAPIdpRepository : Repository<AuthDbContext, LDAPIdp, Guid>, ILDAPIdpRepository
{
    public LDAPIdpRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
