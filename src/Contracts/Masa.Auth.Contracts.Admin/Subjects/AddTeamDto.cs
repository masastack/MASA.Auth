namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddTeamDto
{
    public string Name { get; set; }

    public AvatarValueDto Avatar { get; set; }

    public string Describe { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> AdminStaffs { get; set; }

    public List<Guid> AdminPermissions { get; set; }

    public List<Guid> AdminRoles { get; set; }

    public List<Guid> MemberStaffs { get; set; }

    public List<Guid> MemberPermissions { get; set; }

    public List<Guid> MemberRoles { get; set; }

    public AddTeamDto(string name, AvatarValueDto avatar, string describe, TeamTypes teamType, List<Guid> adminStaffs, List<Guid> adminPermissions, List<Guid> adminRoles, List<Guid> memberStaffs, List<Guid> memberPermissions, List<Guid> memberRoles)
    {
        Name = name;
        Avatar = avatar;
        Describe = describe;
        TeamType = teamType;
        AdminStaffs = adminStaffs;
        AdminPermissions = adminPermissions;
        AdminRoles = adminRoles;
        MemberStaffs = memberStaffs;
        MemberPermissions = memberPermissions;
        MemberRoles = memberRoles;
    }

    public static implicit operator AddTeamDto(TeamDetailDto team)
    {
        return new AddTeamDto(team.Name, team.Avatar, team.Describe, team.TeamType, team.AdminStaffs, team.AdminPermissions, team.AdminRoles, team.MemberStaffs, team.MemberPermissions, team.MemberRoles);
    }
}


