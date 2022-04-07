﻿namespace Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

public interface ITeamRepository : IRepository<Team, Guid>
{
    Task<Team> GetByIdAsync(Guid id);
}
