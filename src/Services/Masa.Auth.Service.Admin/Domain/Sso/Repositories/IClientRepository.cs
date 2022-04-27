﻿namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IClientRepository : IRepository<Client, int>
{
    Task<Client> GetByIdAsync(int id);
}
