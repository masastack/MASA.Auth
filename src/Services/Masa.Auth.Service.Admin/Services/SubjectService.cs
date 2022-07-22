// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using SubjectTypes = Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Enum.SubjectTypes;

namespace Masa.Auth.Service.Admin.Services;

public class SubjectService : RestServiceBase
{
    public SubjectService(IServiceCollection services) : base(services, "api/subject")
    {
    }

    private async Task<List<SubjectModel>> GetListAsync([FromServices] IEventBus eventBus, [FromQuery] string filter)
    {
        var result = new List<SubjectModel>();
        //user
        var userQuery = new UserSelectQuery(filter);
        await eventBus.PublishAsync(userQuery);
        var users = userQuery.Result.Select(user => new SubjectModel(user.UserId, user.Name, user.DisplayName, user.Avatar, user.PhoneNumber, user.Email, SubjectTypes.User));
        //department
        var departmentQuery = new DepartmentSelectQuery(filter);
        await eventBus.PublishAsync(departmentQuery);
        var departments = departmentQuery.Result.Select(department => new SubjectModel(department.Id, department.Name, department.Name, "", "", "", SubjectTypes.Department));
        //team
        var teamQuery = new TeamSelectListQuery(filter);
        await eventBus.PublishAsync(teamQuery);
        var teams = teamQuery.Result.Select(team => new SubjectModel(team.Id, team.Name, team.Name, team.Avatar, "", "", SubjectTypes.Team));
        //role
        var roleQuery = new RoleSelectQuery(filter);
        await eventBus.PublishAsync(roleQuery);
        var roles = roleQuery.Result.Select(role => new SubjectModel(role.Id, role.Name, role.Name, "", "", "", SubjectTypes.Role));

        result.AddRange(users);
        result.AddRange(departments);
        result.AddRange(teams);
        result.AddRange(roles);

        return result;
    }
}
