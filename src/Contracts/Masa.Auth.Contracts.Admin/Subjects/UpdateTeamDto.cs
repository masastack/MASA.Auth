namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateTeamDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public AvatarValueDto Avatar { get; set; }

    public string Description { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> AdminStaffs { get; set; }

    public List<Guid> AdminPermissions { get; set; }

    public List<Guid> AdminRoles { get; set; }

    public List<Guid> MemberStaffs { get; set; }

    public List<Guid> MemberPermissions { get; set; }

    public List<Guid> MemberRoles { get; set; }

    public UpdateTeamDto(Guid id, string name, AvatarValueDto avatar, string description, TeamTypes teamType, List<Guid> adminStaffs, List<Guid> adminPermissions, List<Guid> adminRoles, List<Guid> memberStaffs, List<Guid> memberPermissions, List<Guid> memberRoles)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Description = description;
        TeamType = teamType;
        AdminStaffs = adminStaffs;
        AdminPermissions = adminPermissions;
        AdminRoles = adminRoles;
        MemberStaffs = memberStaffs;
        MemberPermissions = memberPermissions;
        MemberRoles = memberRoles;
    }

    public static implicit operator UpdateTeamDto(TeamDetailDto team)
    {
        return new UpdateTeamDto(team.Id, team.Name, team.Avatar, team.Describe, team.TeamType, team.AdminStaffs, team.AdminPermissions, team.AdminRoles, team.MemberStaffs, team.MemberPermissions, team.MemberRoles);
    }
}


