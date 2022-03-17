namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class TeamDetailResponse
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public AvatarValue Avatar { get; set; }

    public string Describe { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> AdminStaffs { get; set; }

    public List<Guid> AdminPermissions { get; set; }

    public List<Guid> AdminRoles { get; set; }

    public List<Guid> MemberStaffs { get; set; }

    public List<Guid> MemberPermissions { get; set; }

    public List<Guid> MemberRoles { get; set; }

    public static TeamDetailResponse Default => new(Guid.Empty, "", new(), "", default, new(), new(), new(), new(), new(), new());

    public TeamDetailResponse(Guid teamId, string name, AvatarValue avatar, string describe, TeamTypes teamType, List<Guid> adminStaffs, List<Guid> adminPermissions, List<Guid> adminRoles, List<Guid> memberStaffs, List<Guid> memberPermissions, List<Guid> memberRoles)
    {
        TeamId = teamId;
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
}


