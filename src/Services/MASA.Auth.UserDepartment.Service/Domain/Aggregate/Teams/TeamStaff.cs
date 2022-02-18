namespace MASA.Auth.UserDepartment.Domain.Aggregate.Teams
{
    public class TeamStaff
    {
        public Guid TeamId { get; set; }
        
        public Guid StaffId { get; set; }
    }

    public enum TeamStaffType
    {
        Member,
        Admin,
    }
}
