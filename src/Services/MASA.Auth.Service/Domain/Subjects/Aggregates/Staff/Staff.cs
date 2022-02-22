using System.ComponentModel.DataAnnotations;

namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class Staff : Entity<Guid>
    {
        public Guid PlatformId { get; private set; }

        public Guid UserId { get; private set; }

        public string Account { get; private set; }

        public string? Password { get; private set; }

        public string? Avatar { get; private set; }

        public UserState UserState { get; private set; }

        public string? FlowerName { get; private set; }

        public string? JobNumber { get; private set; }

        public StaffType StaffType { get; private set; }

        public StaffState StaffState { get; private set; }

        private Staff()
        {
            Account = "";
        }

        public Staff(Guid platformId, Guid userId, string account, string? password, string? avatar, UserState userState, string? flowerName, string? jobNumber, StaffType staffType, StaffState staffState)
        {
            PlatformId = platformId;
            UserId = userId;
            Account = account;
            Password = password;
            Avatar = avatar;
            UserState = userState;
            FlowerName = flowerName;
            JobNumber = jobNumber;
            StaffType = staffType;
            StaffState = staffState;
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
