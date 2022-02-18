using MASA.Auth.UserDepartment.Domain.Aggregate.Platform;

namespace MASA.Auth.UserDepartment.Domain.Aggregate.Staff
{
    /// <summary>
    /// Staff
    /// <para>Staff Implicitly belongs to the Auth platform</para> 
    /// </summary>
    public class Staff : AuditAggregateRoot<Guid, Guid>
    {
        public Guid UserId { get; set; }

        public string Account { get; set; }

        public string? Password { get; set; }

        public UserState UserState { get; set; }

        public string? FlowerName { get; set; }

        public string? JobNumber { get; set; }

        public StaffType StaffType { get; set; }

        public StaffState StaffState { get; set; }

        public DateOnly? OnboardingTime { get; set; }

        /// <summary>
        /// unit yuan
        /// </summary>
        public int Salary { get; set; }

        public Staff(string account) => Account = account;
    }
}

public enum StaffType
{
    InternalStaff,
    ExternalStaff
}

public enum StaffState
{
    OnDuty,
    LeaveDuty,
    Vacation
}
