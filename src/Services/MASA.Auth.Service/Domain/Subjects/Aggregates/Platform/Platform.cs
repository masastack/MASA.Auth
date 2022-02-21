namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class Platform : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        public string Url { get; private set; }

        public string Icon { get; private set; }

        public PlatformType PlatformType { get; private set; }

        public VerifyType VerifyType { get; private set; }

        public IdentificationType IdentificationType { get; private set; }

        private Platform()
        {
            Name = "";
            DisplayName = "";
            ClientId = "";
            ClientSecret = "";
            Url = "";
            Icon = "";
        }

        public Platform(string name, string displayName, string clientId, string clientSecret, string url, string icon, PlatformType platformType, VerifyType verifyType, IdentificationType identificationType)
        {
            Name = name;
            DisplayName = displayName;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Url = url;
            Icon = icon;
            PlatformType = platformType;
            VerifyType = verifyType;
            IdentificationType = identificationType;
        }
    }

    public enum PlatformType
    {
        ThirdParty,
        Private
    }

    public enum IdentificationType
    {
        MobilePhone,
        Email
    }

    public enum VerifyType
    {
        OAuth
    }
}
