namespace MASA.Auth.UserDepartment.Domain.Aggregate
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
