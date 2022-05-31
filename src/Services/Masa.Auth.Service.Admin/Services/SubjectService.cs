// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class SubjectService : RestServiceBase
{
    public SubjectService(IServiceCollection services) : base(services, "api/subject")
    {
    }

    private async Task<List<SubjectDto>> GetListAsync([FromServices] IEventBus eventBus, [FromQuery] string filter)
    {
        var result = new List<SubjectDto>();
        //user
        var userQuery = new UserSelectQuery(filter);
        await eventBus.PublishAsync(userQuery);
        var users = userQuery.Result.Select(user => new SubjectDto(user.Id, user.Name, user.DisplayName, user.Avatar, user.PhoneNumber, user.Email, BusinessTypes.User));
        //department
        var departmentQuery = new DepartmentSelectQuery(filter);
        await eventBus.PublishAsync(departmentQuery);
        var departments = departmentQuery.Result.Select(department => new SubjectDto(department.Id, department.Name, department.Name, "", "", "", BusinessTypes.Department));
        //team
        var teamQuery = new TeamSelectListQuery(filter);
        await eventBus.PublishAsync(teamQuery);
        var teams = teamQuery.Result.Select(team => new SubjectDto(team.Id, team.Name, team.Name, team.Avatar, "", "", BusinessTypes.Team));
        //role
        var roleQuery = new RoleSelectQuery(filter);
        await eventBus.PublishAsync(teamQuery);
        var roles = teamQuery.Result.Select(team => new SubjectDto(team.Id, team.Name, team.Name, team.Avatar, "", "", BusinessTypes.Team));

        result.AddRange(users);
        result.AddRange(departments);
        result.AddRange(teams);
        result.AddRange(roles);

        return result;
    }
}
