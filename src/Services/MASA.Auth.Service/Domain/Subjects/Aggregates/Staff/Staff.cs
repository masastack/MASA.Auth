namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class Staff : PlatformUser
    {
        public string? FlowerName { get; private set; }

        public string? JobNumber { get; private set; }

        public StaffType StaffType { get; private set; }

        public StaffState StaffState { get; private set; }

        public DateTime? OnboardingTime { get; private set; }

        /// <summary>
        /// unit yuan
        /// </summary>
        public int Salary { get; private set; }

        private Staff() : base()
        {

        }

        public Staff(Guid platformId, Guid userId, string account, string? password, string? avatar, UserState userState, string? flowerName, string? jobNumber, StaffType staffType, StaffState staffState, DateTime? onboardingTime, int salary) : base(platformId, userId, account, password, avatar, userState)
        {
            FlowerName = flowerName;
            JobNumber = jobNumber;
            StaffType = staffType;
            StaffState = staffState;
            OnboardingTime = onboardingTime;
            Salary = salary;
        }
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
