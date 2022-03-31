namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddTeamDto
{
    public string Name { get; set; } = string.Empty;

    public AvatarValueDto Avatar { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    public TeamTypes Type { get; set; } = TeamTypes.Normal;

    public List<Guid> AdminStaffs { get; set; } = new();

    public List<Guid> AdminRoles { get; set; } = new();

    public Dictionary<Guid, bool> AdminPermissions { get; set; } = new();

    public List<Guid> MemberStaffs { get; set; } = new();

    public List<Guid> MemberRoles { get; set; } = new();

    public Dictionary<Guid, bool> MemberPermissions { get; set; } = new();

    public static implicit operator AddTeamDto(TeamDetailDto team)
    {
        return new AddTeamDto()
        {
            Name = team.TeamBaseInfo.Name,
            Description = team.TeamBaseInfo.Description,
            Type = (TeamTypes)team.TeamBaseInfo.Type,
            Avatar = team.TeamBaseInfo.Avatar,
            AdminStaffs = team.TeamAdmin.Staffs,
            AdminRoles = team.TeamAdmin.Roles,
            AdminPermissions = team.TeamAdmin.Permissions,
            MemberRoles = team.TeamMember.Roles,
            MemberStaffs = team.TeamMember.Staffs,
            MemberPermissions = team.TeamMember.Permissions
        };
    }
}
